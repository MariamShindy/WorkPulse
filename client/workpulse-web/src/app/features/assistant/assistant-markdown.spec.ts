import { renderAssistantMarkdown } from './assistant-markdown';

describe('renderAssistantMarkdown', () => {
  it('returns empty output for blank input', () => {
    expect(renderAssistantMarkdown('')).toBe('');
    expect(renderAssistantMarkdown('   ')).toBe('');
  });

  it('escapes raw html before rendering markdown', () => {
    const html = renderAssistantMarkdown('<script>alert(1)</script>');
    expect(html).not.toContain('<script>');
    expect(html).toContain('&lt;script&gt;alert(1)&lt;/script&gt;');
  });

  it('renders headings, lists, and inline emphasis', () => {
    const html = renderAssistantMarkdown('# Title\n\n- **Open**: 3 tasks\n\nPlain paragraph.');

    expect(html).toContain('<h1 class="md-h">Title</h1>');
    expect(html).toContain('<ul class="md-list"><li><strong>Open</strong>: 3 tasks</li></ul>');
    expect(html).toContain('<p class="md-p">Plain paragraph.</p>');
  });

  it('renders markdown tables', () => {
    const html = renderAssistantMarkdown(
      '| Task | Owner |\n| --- | --- |\n| ENG-1 | Alice |'
    );

    expect(html).toContain('<table class="md-table">');
    expect(html).toContain('<th>Task</th>');
    expect(html).toContain('<td>ENG-1</td>');
    expect(html).toContain('<td>Alice</td>');
  });
});
