import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { renderAssistantMarkdown } from './assistant-markdown';

@Pipe({ name: 'assistantMarkdown', standalone: true })
export class AssistantMarkdownPipe implements PipeTransform {
  constructor(private readonly sanitizer: DomSanitizer) {}

  transform(value: string | null | undefined): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(renderAssistantMarkdown(value ?? ''));
  }
}
