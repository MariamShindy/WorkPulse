import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ReportsService } from './reports.service';

describe('ReportsService', () => {
  let service: ReportsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ReportsService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(ReportsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('requests the task summary with filters', () => {
    service.taskSummary({ teamId: 't1', from: '2026-01-01' }).subscribe();

    const req = httpMock.expectOne((r) => r.url === '/api/reports/task-summary');
    expect(req.request.params.get('teamId')).toBe('t1');
    expect(req.request.params.get('from')).toBe('2026-01-01');
    req.flush({ generatedAtUtc: '', totalTasks: 0, openTasks: 0, completedTasks: 0, overdueTasks: 0, rows: [] });
  });

  it('downloads the CSV export as a blob', () => {
    service.exportTasksCsv({}).subscribe((response) => {
      expect(response.body instanceof Blob).toBeTrue();
    });

    const req = httpMock.expectOne('/api/reports/export/tasks/csv');
    expect(req.request.responseType).toBe('blob');
    req.flush(new Blob(['id,title'], { type: 'text/csv' }));
  });
});
