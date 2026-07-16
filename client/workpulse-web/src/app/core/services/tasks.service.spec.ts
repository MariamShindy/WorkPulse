import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TasksService } from './tasks.service';

describe('TasksService', () => {
  let service: TasksService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TasksService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(TasksService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('lists tasks with filters as query params', () => {
    service.list({ teamId: 't1', projectId: 'p1', priority: 'High', page: 2, pageSize: 10 }).subscribe();

    const req = httpMock.expectOne(
      (r) => r.url === '/api/tasks' && r.params.get('teamId') === 't1'
    );
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('projectId')).toBe('p1');
    expect(req.request.params.get('priority')).toBe('High');
    expect(req.request.params.get('page')).toBe('2');
    req.flush({ items: [], page: 2, pageSize: 10, totalCount: 0, totalPages: 0, hasPreviousPage: true, hasNextPage: false });
  });

  it('sends priority as its numeric enum value when creating', () => {
    service.create('team-1', { title: 'Task', priority: 'High' }).subscribe();

    const req = httpMock.expectOne('/api/tasks');
    expect(req.request.method).toBe('POST');
    expect(req.request.body.priority).toBe(3);
    expect(req.request.body.teamId).toBe('team-1');
    expect(req.request.body.isBlocked).toBeFalse();
    req.flush({});
  });

  it('moves a task to a workflow state', () => {
    service.move('task-1', 'state-2', 5).subscribe();

    const req = httpMock.expectOne('/api/tasks/task-1/move');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ workflowStateId: 'state-2', sortOrder: 5 });
    req.flush({});
  });

  it('manages assignees, labels and dependencies via nested routes', () => {
    service.addAssignee('task-1', 'user-1').subscribe();
    httpMock.expectOne('/api/tasks/task-1/assignees').flush(null);

    service.assignLabel('task-1', 'label-1').subscribe();
    httpMock.expectOne('/api/tasks/task-1/labels').flush(null);

    service.addDependency('task-1', 'task-2', 'BlockedBy').subscribe();
    const depReq = httpMock.expectOne('/api/tasks/task-1/dependencies');
    expect(depReq.request.body.type).toBe(1);
    depReq.flush({});

    service.removeAssignee('task-1', 'user-1').subscribe();
    const delReq = httpMock.expectOne('/api/tasks/task-1/assignees/user-1');
    expect(delReq.request.method).toBe('DELETE');
    delReq.flush(null);
  });
});
