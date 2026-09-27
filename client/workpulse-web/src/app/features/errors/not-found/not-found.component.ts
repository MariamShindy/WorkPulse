import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [RouterLink],
  template: `
    <section class="not-found">
      <p class="code" aria-hidden="true">404</p>
      <h1>We couldn't find that page</h1>
      <p class="lede">
        The link may be out of date, or the item may have been moved or deleted.
      </p>
      <a routerLink="/dashboard" class="btn btn-primary">Back to dashboard</a>
    </section>
  `,
  styles: `
    .not-found {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: var(--sp-3);
      min-height: 60vh;
      text-align: center;
      padding: var(--sp-6);
    }

    .code {
      margin: 0;
      font-size: 4rem;
      font-weight: 700;
      line-height: 1;
      letter-spacing: -0.04em;
      color: var(--accent);
      opacity: 0.65;
    }

    h1 {
      margin: 0;
      font-size: var(--font-xl);
      font-weight: 650;
      letter-spacing: -0.02em;
    }

    .lede {
      margin: 0;
      max-width: 34rem;
      color: var(--text-muted);
    }

    a.btn {
      margin-top: var(--sp-2);
    }
  `
})
export class NotFoundComponent {}
