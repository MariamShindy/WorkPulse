import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ProjectsService } from './projects.service';

describe('ProjectsService', () => {
  let service: ProjectsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ProjectsService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(ProjectsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('lists projects filtered by team', () => {
    service.list({ teamId: 'team-1', includeArchived: true }).subscribe();

    const req = httpMock.expectOne((r) => r.url === '/api/projects');
    expect(req.request.params.get('teamId')).toBe('team-1');
    expect(req.request.params.get('includeArchived')).toBe('true');
    expect(req.request.params.get('archivedOnly')).toBe('false');
    req.flush({ items: [], page: 1, pageSize: 50, totalCount: 0, totalPages: 0, hasPreviousPage: false, hasNextPage: false });
  });

  it('lists archived-only projects', () => {
    service.list({ archivedOnly: true, status: 'Active' }).subscribe();

    const req = httpMock.expectOne((r) => r.url === '/api/projects');
    expect(req.request.params.get('includeArchived')).toBe('true');
    expect(req.request.params.get('archivedOnly')).toBe('true');
    expect(req.request.params.get('status')).toBe('1');
    req.flush({ items: [], page: 1, pageSize: 50, totalCount: 0, totalPages: 0, hasPreviousPage: false, hasNextPage: false });
  });

  it('creates a project with a numeric status enum', () => {
    service.create('team-1', { name: 'Website', status: 'Active', key: 'WEB' }).subscribe();

    const req = httpMock.expectOne('/api/projects');
    expect(req.request.method).toBe('POST');
    expect(req.request.body.status).toBe(1);
    expect(req.request.body.teamId).toBe('team-1');
    req.flush({});
  });

  it('archives a project', () => {
    service.archive('p1').subscribe();

    const req = httpMock.expectOne('/api/projects/p1/archive');
    expect(req.request.method).toBe('POST');
    req.flush(null);
  });
});
