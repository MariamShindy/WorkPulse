/**
 * Normalizes ASP.NET ProblemDetails / legacy error payloads into a user-facing message.
 */
export function apiErrorMessage(err: unknown, fallback = 'Something went wrong.'): string {
  const error = err as { error?: { detail?: string; description?: string; title?: string }; message?: string } | null;
  return (
    error?.error?.detail ||
    error?.error?.description ||
    error?.error?.title ||
    error?.message ||
    fallback
  );
}
