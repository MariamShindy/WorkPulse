#!/usr/bin/env bash
# Live smoke test for the security fixes on this branch.
#
# Each check exercises a fix against a running API and prints PASS/FAIL. These are the
# behaviours that unit tests cannot prove on their own: real HTTP headers, real cookies,
# real rate-limit and lockout state.
#
# Usage:  BASE=http://localhost:8080 bash scripts/verify-security-fixes.sh
#
# Requires: curl, python3 (or python), and a seeded Development database.

set -uo pipefail

BASE="${BASE:-http://localhost:8080}"
EMAIL="${EMAIL:-demo@workpulse.local}"
PASSWORD="${PASSWORD:-Demo1234!}"

WORK="$(mktemp -d)"
trap 'rm -rf "$WORK"' EXIT

# curl on Windows needs a native path for -F file uploads.
winpath() { command -v cygpath >/dev/null 2>&1 && cygpath -m "$1" || printf '%s' "$1"; }

# Pick an interpreter that actually runs. On Windows `python3` often resolves to the
# Microsoft Store stub, which exists on PATH but exits non-zero for every invocation.
PY=""
for candidate in python3 python py; do
  if command -v "$candidate" >/dev/null 2>&1 && "$candidate" -c 'pass' >/dev/null 2>&1; then
    PY="$candidate"; break
  fi
done
if [ -z "$PY" ]; then
  echo "No working python found on PATH; needed to read JSON responses." >&2
  exit 1
fi

pass=0; fail=0
ok()   { printf '  \033[32mPASS\033[0m  %s\n' "$1"; pass=$((pass+1)); }
bad()  { printf '  \033[31mFAIL\033[0m  %s\n' "$1"; fail=$((fail+1)); }
head2() { printf '\n\033[1m%s\033[0m\n' "$1"; }

# Asserts that "$1" (actual) contains "$2" (expected substring).
expect_contains() {
  case "$1" in *"$2"*) ok "$3";; *) bad "$3 (got: $(printf '%s' "$1" | head -c 120))";; esac
}

# Reads a dotted key path out of a JSON file: jsonf resp.json user.currentTenantId
# Prints nothing (and exits non-zero) when the path is absent, so callers can test for it.
jsonf() {
  "$PY" - "$1" "$2" <<'PY' 2>/dev/null
import json, sys
with open(sys.argv[1], encoding="utf-8") as fh:
    node = json.load(fh)
for part in sys.argv[2].split("."):
    if part == "":
        continue
    if part.isdigit() and isinstance(node, list):
        node = node[int(part)]
    else:
        node = node[part]
print(",".join(node) if isinstance(node, dict) else node)
PY
}

probe_auth() {
  curl -s -o /dev/null -w '%{http_code}' -X POST "$BASE/api/auth/refresh" \
    -H 'Content-Type: application/json' -d '{}'
}

# Waits until at least one auth permit is available.
wait_for_auth_window() {
  local waited=0 announced=""
  while [ "$waited" -lt 90 ]; do
    [ "$(probe_auth)" != "429" ] && return 0
    [ -z "$announced" ] && { printf '  (waiting for the auth rate-limit window…)\n'; announced=1; }
    sleep 5; waited=$((waited+5))
  done
  printf '  (gave up waiting for the rate-limit window)\n'
}

# Waits for the *start* of a fresh window, so the caller gets the full permit allowance.
#
# wait_for_auth_window alone is not enough for the lockout check: it returns as soon as a
# single permit is free, which may be the last one in a window that earlier sections already
# spent. Draining first, then waiting for the reset, guarantees a full budget — otherwise a
# 429 cuts the bad-password sequence short and lockout looks broken when it is not.
fresh_auth_window() {
  local i=0
  while [ "$i" -lt 20 ]; do
    [ "$(probe_auth)" = "429" ] && break
    i=$((i+1))
  done
  printf '  (waiting for a fresh auth rate-limit window…)\n'
  local waited=0
  while [ "$waited" -lt 90 ]; do
    sleep 5; waited=$((waited+5))
    [ "$(probe_auth)" != "429" ] && return 0
  done
  printf '  (gave up waiting for a fresh window)\n'
}

head2 "0. API reachable"
if ! curl -sf -o /dev/null "$BASE/health"; then
  printf '  API not reachable at %s — start it first (see the guide).\n' "$BASE"
  exit 1
fi
ok "health endpoint responds"

# ─────────────────────────────────────────────────────────────────────────────
head2 "1. Security headers (fix #9)"
H=$(curl -s -D- -o /dev/null "$BASE/health")
expect_contains "$H" "nosniff"                      "X-Content-Type-Options: nosniff"
expect_contains "$H" "X-Frame-Options: DENY"        "X-Frame-Options: DENY"
expect_contains "$H" "default-src 'none'"           "Content-Security-Policy locked down"
expect_contains "$H" "Referrer-Policy: no-referrer" "Referrer-Policy: no-referrer"

# ─────────────────────────────────────────────────────────────────────────────
head2 "2. Refresh token in an HttpOnly cookie (frontend fix #10)"
wait_for_auth_window
JAR="$WORK/jar.txt"
LOGIN_HEADERS=$(curl -s -D- -c "$JAR" -o "$WORK/login.json" \
  -X POST "$BASE/api/auth/login" -H 'Content-Type: application/json' \
  -d "{\"email\":\"$EMAIL\",\"password\":\"$PASSWORD\"}")

if ! grep -q 200 <<<"$(head -1 <<<"$LOGIN_HEADERS")"; then
  printf '  Login failed — is the dev seed present? Response:\n'
  cat "$WORK/login.json"; exit 1
fi

expect_contains "$LOGIN_HEADERS" "httponly"          "Set-Cookie is HttpOnly"
expect_contains "$LOGIN_HEADERS" "samesite=strict"   "Set-Cookie is SameSite=Strict"
expect_contains "$LOGIN_HEADERS" "path=/api/auth"    "cookie scoped to /api/auth"

KEYS=$(jsonf "$WORK/login.json" "")
case "$KEYS" in
  *refreshToken*) bad "response body still leaks refreshToken";;
  *) ok "response body carries no refreshToken ($KEYS)";;
esac

TOKEN=$(jsonf "$WORK/login.json" "accessToken")
TENANT=$(jsonf "$WORK/login.json" "user.currentTenantId")
AUTH=(-H "Authorization: Bearer $TOKEN" -H "X-Tenant-Id: $TENANT")

# ─────────────────────────────────────────────────────────────────────────────
head2 "3. Refresh works from the cookie alone, and rotates"
REFRESH_TOKEN=$(grep workpulse_refresh "$JAR" | awk '{print $7}')

CODE=$(curl -s -o /dev/null -w '%{http_code}' -X POST "$BASE/api/auth/refresh" \
  -H 'Content-Type: application/json' -H "Cookie: workpulse_refresh=$REFRESH_TOKEN" -d '{}')
[ "$CODE" = "200" ] && ok "refresh with cookie only -> 200" || bad "refresh with cookie -> $CODE"

# Same token again: rotation should have revoked it.
BODY=$(curl -s -X POST "$BASE/api/auth/refresh" \
  -H 'Content-Type: application/json' -H "Cookie: workpulse_refresh=$REFRESH_TOKEN" -d '{}')
expect_contains "$BODY" "Auth.InvalidRefreshToken" "replaying a used refresh token is rejected"

BODY=$(curl -s -X POST "$BASE/api/auth/refresh" -H 'Content-Type: application/json' -d '{}')
expect_contains "$BODY" "Auth.MissingRefreshToken" "refresh with no cookie -> 401, not 400"

# ─────────────────────────────────────────────────────────────────────────────
head2 "4. Pagination clamping (fix #3)"
SIZE=$(curl -s "$BASE/api/tasks?pageSize=10000000" "${AUTH[@]}" > "$WORK/p1.json" && jsonf "$WORK/p1.json" "pageSize")
[ "$SIZE" = "100" ] && ok "pageSize=10000000 clamped to 100" || bad "pageSize=10000000 -> $SIZE (expected 100)"

SIZE=$(curl -s "$BASE/api/tasks?pageSize=0" "${AUTH[@]}" > "$WORK/p2.json" && jsonf "$WORK/p2.json" "pageSize")
[ "$SIZE" = "25" ] && ok "pageSize=0 falls back to 25 (no divide-by-zero)" || bad "pageSize=0 -> $SIZE (expected 25)"

CODE=$(curl -s -o /dev/null -w '%{http_code}' "$BASE/api/tasks?page=-5" "${AUTH[@]}")
[ "$CODE" = "200" ] && ok "page=-5 handled (no negative skip)" || bad "page=-5 -> $CODE"

# ─────────────────────────────────────────────────────────────────────────────
head2 "5. Upload content-type spoofing (fix #6)"
TASK_ID=$(jsonf "$WORK/p1.json" "items.0.id")
if [ -z "${TASK_ID:-}" ]; then
  printf '  skipped: no tasks in the seeded workspace\n'
else
  printf '<html><script>alert(1)</script></html>' > "$WORK/evil.png"
  printf 'MZ\x90\x00\x03\x00\x00\x00'             > "$WORK/evil.pdf"
  printf '\x89PNG\r\n\x1a\n\x00\x00\x00\rIHDR\x00\x00\x00\x01\x00\x00\x00\x01\x08\x06\x00\x00\x00' > "$WORK/real.png"
  W=$(winpath "$WORK")

  upload() { curl -s -X POST "$BASE/api/files/upload" "${AUTH[@]}" \
    -F "file=@$W/$1;type=$2" -F "entityType=0" -F "entityId=$TASK_ID"; }

  expect_contains "$(upload evil.png image/png)"       "Files.ContentMismatch" "HTML declared as image/png rejected"
  expect_contains "$(upload evil.pdf application/pdf)" "Files.ContentMismatch" "EXE declared as application/pdf rejected"

  GOOD=$(upload real.png image/png)
  expect_contains "$GOOD" '"contentType":"image/png"' "genuine PNG still accepted"

  # Fix #7: the URL handed back must be the route that actually exists.
  expect_contains "$GOOD" '/download' "publicUrl points at a real route"
  FID=$(printf '%s' "$GOOD" | "$PY" -c "import json,sys;print(json.load(sys.stdin).get('id',''))" 2>/dev/null)
  if [ -n "$FID" ]; then
    CODE=$(curl -s -o /dev/null -w '%{http_code}' "$BASE/api/files/$FID/download" "${AUTH[@]}")
    [ "$CODE" = "200" ] && ok "returned download URL resolves (200)" || bad "download URL -> $CODE"
  fi

  # Fix found during review: CreatedAtUtc was default(DateTime) in the response.
  case "$GOOD" in
    *'"createdAtUtc":"0001-'*) bad "createdAtUtc still returns year 0001";;
    *) ok "createdAtUtc is a real timestamp";;
  esac
fi

# ─────────────────────────────────────────────────────────────────────────────
head2 "6. Account lockout actually engages (fix #8)"
fresh_auth_window
LOCK_EMAIL="lockout-probe-$RANDOM@workpulse.local"
curl -s -o /dev/null -X POST "$BASE/api/auth/register" -H 'Content-Type: application/json' \
  -d "{\"email\":\"$LOCK_EMAIL\",\"password\":\"$PASSWORD\",\"firstName\":\"Lock\",\"lastName\":\"Probe\"}"

locked=""
for _ in 1 2 3 4 5; do
  R=$(curl -s -X POST "$BASE/api/auth/login" -H 'Content-Type: application/json' \
    -d "{\"email\":\"$LOCK_EMAIL\",\"password\":\"DefinitelyWrong1!\"}")
  case "$R" in *Auth.AccountLocked*) locked=yes; break;; esac
done
[ -n "$locked" ] && ok "repeated bad passwords lock the account" || bad "account never locked (lockout still bypassed?)"

# The correct password must also be refused while locked.
R=$(curl -s -X POST "$BASE/api/auth/login" -H 'Content-Type: application/json' \
  -d "{\"email\":\"$LOCK_EMAIL\",\"password\":\"$PASSWORD\"}")
expect_contains "$R" "Auth.AccountLocked" "correct password refused while locked"

# ─────────────────────────────────────────────────────────────────────────────
head2 "7. Rate limiting (fix #4)"
wait_for_auth_window
# Runs last: it deliberately exhausts the auth window for this client.
limited=""
for _ in $(seq 1 15); do
  R=$(curl -s -D- -o /dev/null -X POST "$BASE/api/auth/login" \
    -H 'Content-Type: application/json' -d '{"email":"nobody@example.com","password":"x"}')
  case "$R" in *429*) limited="$R"; break;; esac
done
if [ -n "$limited" ]; then
  ok "login is rate limited (429)"
  expect_contains "$limited" "Retry-After" "429 carries Retry-After"
else
  bad "no 429 after 15 login attempts"
fi

printf '\n\033[1m%d passed, %d failed\033[0m\n' "$pass" "$fail"
[ "$fail" -eq 0 ] || exit 1
