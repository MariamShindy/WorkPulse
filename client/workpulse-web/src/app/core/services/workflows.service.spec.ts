import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { WorkflowsService } from './workflows.service';

describe('WorkflowsService', () => {
  let service: WorkflowsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [WorkflowsService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(WorkflowsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('creates a state with numeric type under the team workflow route', () => {
    service.createState('team-1', { name: 'QA', type: 'Started', color: '#fff', position: 2 }).subscribe();

    const req = httpMock.expectOne('/api/teams/team-1/workflow/states');
    expect(req.request.method).toBe('POST');
    expect(req.request.body.type).toBe(2);
    req.flush({});
  });

  it('reorders states', () => {
    service.reorder('team-1', ['b', 'a']).subscribe();

    const req = httpMock.expectOne('/api/teams/team-1/workflow/states/reorder');
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({ stateIdsInOrder: ['b', 'a'] });
    req.flush(null);
  });
});
