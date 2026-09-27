/**
 * Minimal read-only access to a JWT's payload.
 *
 * The token is *not* verified here — the API is the only thing that can do that. This exists so
 * the client can avoid routing a user into the app with an access token that already expired,
 * which previously produced a dashboard full of 401s instead of a redirect to login.
 */
interface JwtPayload {
  exp?: number;
  sub?: string;
}

function decodePayload(token: string): JwtPayload | null {
  const parts = token.split('.');
  if (parts.length !== 3) return null;

  try {
    // Base64url -> base64, then pad to a multiple of 4.
    const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), '=');

    // atob yields Latin-1; percent-decode so non-ASCII claims survive.
    const json = decodeURIComponent(
      atob(padded)
        .split('')
        .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
        .join('')
    );

    return JSON.parse(json) as JwtPayload;
  } catch {
    return null;
  }
}

/** Expiry as epoch milliseconds, or null when the token carries no readable `exp`. */
export function jwtExpiresAt(token: string): number | null {
  const exp = decodePayload(token)?.exp;
  return typeof exp === 'number' ? exp * 1000 : null;
}

/**
 * True when the token is past its `exp`. A token with no readable expiry is treated as valid so
 * a format we cannot parse degrades to the previous behaviour (let the API decide) rather than
 * logging everyone out.
 *
 * @param skewMs Treat a token expiring within this window as already expired, so a request is
 *               not fired off microseconds before it lapses. The API uses ClockSkew.Zero.
 */
export function isJwtExpired(token: string | null, skewMs = 5_000): boolean {
  if (!token) return true;

  const expiresAt = jwtExpiresAt(token);
  if (expiresAt === null) return false;

  return Date.now() + skewMs >= expiresAt;
}
