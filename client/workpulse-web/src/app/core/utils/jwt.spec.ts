import { isJwtExpired, jwtExpiresAt } from './jwt';

/** Builds an unsigned token whose payload carries the given claims. */
function tokenWith(payload: Record<string, unknown>): string {
  const encode = (o: unknown) => {
    // btoa only accepts Latin-1, so percent-encode to UTF-8 bytes first — the same round
    // trip the decoder under test performs in reverse.
    const utf8 = encodeURIComponent(JSON.stringify(o)).replace(/%([0-9A-F]{2})/g, (_, hex) =>
      String.fromCharCode(parseInt(hex, 16))
    );
    return btoa(utf8).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
  };
  return `${encode({ alg: 'HS256', typ: 'JWT' })}.${encode(payload)}.signature`;
}

describe('jwt helpers', () => {
  const nowSeconds = () => Math.floor(Date.now() / 1000);

  it('reads exp as epoch milliseconds', () => {
    const exp = nowSeconds() + 600;
    expect(jwtExpiresAt(tokenWith({ exp }))).toBe(exp * 1000);
  });

  it('returns null when there is no exp claim', () => {
    expect(jwtExpiresAt(tokenWith({ sub: 'abc' }))).toBeNull();
  });

  it('treats a future token as valid', () => {
    expect(isJwtExpired(tokenWith({ exp: nowSeconds() + 3600 }))).toBe(false);
  });

  it('treats a past token as expired', () => {
    expect(isJwtExpired(tokenWith({ exp: nowSeconds() - 60 }))).toBe(true);
  });

  it('treats a token expiring inside the skew window as expired', () => {
    // The API validates with ClockSkew.Zero, so a token with 2s left is not worth sending.
    expect(isJwtExpired(tokenWith({ exp: nowSeconds() + 2 }), 5_000)).toBe(true);
  });

  it('treats a null token as expired', () => {
    expect(isJwtExpired(null)).toBe(true);
  });

  it('does not log the user out over a token it cannot parse', () => {
    // Degrade to the old behaviour — let the API be the judge — rather than forcing a logout.
    expect(isJwtExpired('not-a-jwt')).toBe(false);
    expect(isJwtExpired('a.b')).toBe(false);
    expect(isJwtExpired(tokenWith({ sub: 'no-exp' }))).toBe(false);
  });

  it('handles non-ascii claims without throwing', () => {
    const token = tokenWith({ exp: nowSeconds() + 600, name: 'Ünïcodé 世界' });
    expect(isJwtExpired(token)).toBe(false);
  });
});
