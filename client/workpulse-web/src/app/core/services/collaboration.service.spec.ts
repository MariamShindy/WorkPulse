import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { CollaborationService } from './collaboration.service';

describe('CollaborationService', () => {
  let service: CollaborationService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CollaborationService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(CollaborationService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('adds a comment to a task', () => {
    service.addComment('task-1', 'Hello').subscribe();

    const req = httpMock.expectOne('/api/tasks/task-1/comments');
    expect(req.request.method).toBe('POST');
    expect(req.request.body.body).toBe('Hello');
    req.flush({});
  });

  it('lists notifications with unreadOnly flag', () => {
    service.listNotifications(true, 1, 5).subscribe();

    const req = httpMock.expectOne((r) => r.url === '/api/notifications');
    expect(req.request.params.get('unreadOnly')).toBe('true');
    expect(req.request.params.get('pageSize')).toBe('5');
    req.flush({ items: [], page: 1, pageSize: 5, totalCount: 0, totalPages: 0, hasPreviousPage: false, hasNextPage: false });
  });

  it('marks a notification read and all read', () => {
    service.markNotificationRead('n1').subscribe();
    const single = httpMock.expectOne('/api/notifications/n1/read');
    expect(single.request.method).toBe('POST');
    single.flush(null);

    service.markAllNotificationsRead().subscribe();
    httpMock.expectOne('/api/notifications/read-all').flush(null);
  });

  it('watches and unwatches a task', () => {
    service.watch('task-1').subscribe();
    httpMock.expectOne('/api/tasks/task-1/watch').flush(null);

    service.unwatch('task-1').subscribe();
    const req = httpMock.expectOne('/api/tasks/task-1/watch');
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
