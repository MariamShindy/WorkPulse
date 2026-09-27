// ── Shared ───────────────────────────────────────────────────────────────────

export interface PagedList<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ApiError {
  code?: string;
  description?: string;
}

// Enum name lists — index matches the numeric value the API expects in JSON bodies.
export const TASK_PRIORITIES = ['None', 'Low', 'Medium', 'High', 'Urgent'] as const;
export const PROJECT_STATUSES = ['Planned', 'Active', 'Paused', 'Completed', 'Cancelled'] as const;
export const EPIC_STATUSES = ['Open', 'InProgress', 'Done', 'Cancelled'] as const;
export const SPRINT_STATUSES = ['Planned', 'Active', 'Completed', 'Cancelled'] as const;
export const WORKFLOW_STATE_TYPES = ['Backlog', 'Unstarted', 'Started', 'Completed', 'Cancelled'] as const;
export const COMPANY_ROLES = ['Owner', 'Admin', 'Member', 'Guest'] as const;
export const TEAM_ROLES = ['Lead', 'Member'] as const;
export const DEPENDENCY_TYPES = ['Blocks', 'BlockedBy', 'RelatesTo'] as const;
export const FILE_ENTITY_TYPES = ['Task', 'Project', 'Team', 'Comment'] as const;
export const SAVED_VIEW_ENTITY_TYPES = ['Tasks', 'Projects', 'Epics', 'Sprints'] as const;
export const SEARCH_SCOPES = ['All', 'Tasks', 'Projects', 'Teams', 'Comments'] as const;
export const AUTOMATION_TRIGGERS = ['TaskStatusChanged', 'TaskCreated', 'TaskAssigned'] as const;
export const AUTOMATION_ACTIONS = ['SendNotification', 'UpdatePriority', 'AssignUser'] as const;

/** Converts an enum name to the numeric value the API expects in JSON bodies. */
export function enumValue(list: readonly string[], name: string): number {
  const idx = list.indexOf(name);
  return idx >= 0 ? idx : 0;
}

// ── Auth / users ─────────────────────────────────────────────────────────────

export interface UserProfile {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  avatarUrl?: string | null;
  currentTenantId?: string | null;
  createdAtUtc: string;
  lastLoginAtUtc?: string | null;
}

/** The refresh token is delivered as an HttpOnly cookie and is deliberately absent here. */
export interface AuthResponse {
  accessToken: string;
  accessTokenExpiresAtUtc: string;
  user: UserProfile;
}

export interface Invitation {
  id: string;
  email: string;
  role: string;
  status: string;
  expiresAtUtc: string;
  acceptedAtUtc?: string | null;
  createdAtUtc: string;
}

// ── Organizations ────────────────────────────────────────────────────────────

export interface Company {
  id: string;
  name: string;
  slug: string;
  logoUrl?: string | null;
  description?: string | null;
  isActive: boolean;
  createdAtUtc: string;
}

/** A company the current user belongs to, with their membership role. */
export interface UserCompany {
  id: string;
  name: string;
  slug: string;
  logoUrl?: string | null;
  role: string;
}

export interface CompanyMember {
  id: string;
  userId: string;
  email: string;
  fullName: string;
  role: string;
  isActive: boolean;
  createdAtUtc: string;
}

export interface Team {
  id: string;
  name: string;
  key: string;
  description?: string | null;
  icon?: string | null;
  color?: string | null;
  isArchived: boolean;
  createdAtUtc: string;
}

export interface TeamMember {
  id: string;
  teamId: string;
  userId: string;
  email: string;
  fullName: string;
  role: string;
  createdAtUtc: string;
}

// ── Projects / tasks / workflow ──────────────────────────────────────────────

export interface Project {
  id: string;
  teamId: string;
  teamKey: string;
  name: string;
  key: string;
  description?: string | null;
  status: string;
  leadId?: string | null;
  startDate?: string | null;
  targetDate?: string | null;
  isArchived: boolean;
  createdAtUtc: string;
}

export interface TaskItem {
  id: string;
  teamId: string;
  teamKey: string;
  identifier: string;
  projectId?: string | null;
  projectKey?: string | null;
  workflowStateId: string;
  workflowStateName: string;
  workflowStateType: string;
  title: string;
  description?: string | null;
  priority: string;
  assigneeId?: string | null;
  assigneeIds: string[];
  creatorId: string;
  dueDate?: string | null;
  parentTaskId?: string | null;
  sortOrder: number;
  storyPoints?: number | null;
  estimatedHours?: number | null;
  loggedHours: number;
  isBlocked: boolean;
  blockedReason?: string | null;
  epicId?: string | null;
  sprintId?: string | null;
  assignedTeamId?: string | null;
  createdAtUtc: string;
}

export interface TaskAssignee {
  id: string;
  taskId: string;
  userId: string;
  createdAtUtc: string;
}

export interface TaskDependency {
  id: string;
  taskId: string;
  dependsOnTaskId: string;
  type: string;
  createdAtUtc: string;
}

export interface WorkflowState {
  id: string;
  name: string;
  type: string;
  color: string;
  position: number;
  isDefault: boolean;
}

export interface Workflow {
  id: string;
  teamId: string;
  name: string;
  isDefault: boolean;
  states: WorkflowState[];
}

// ── Work management ──────────────────────────────────────────────────────────

export interface Label {
  id: string;
  name: string;
  color: string;
  createdAtUtc: string;
}

export interface Epic {
  id: string;
  teamId: string;
  title: string;
  description?: string | null;
  status: string;
  createdAtUtc: string;
}

export interface Sprint {
  id: string;
  teamId: string;
  name: string;
  goal?: string | null;
  startDate: string;
  endDate: string;
  status: string;
  createdAtUtc: string;
}

export interface WorkLog {
  id: string;
  taskId: string;
  userId: string;
  hours: number;
  description?: string | null;
  loggedDate: string;
  createdAtUtc: string;
}

export interface SavedView {
  id: string;
  userId: string;
  name: string;
  entityType: string;
  filtersJson: string;
  sortJson: string;
  isShared: boolean;
  createdAtUtc: string;
}

// ── Collaboration ────────────────────────────────────────────────────────────

export interface TaskComment {
  id: string;
  taskId: string;
  authorId: string;
  body: string;
  mentionedUserIds: string[];
  isEdited: boolean;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
}

export interface TaskActivity {
  id: string;
  taskId: string;
  actorId: string;
  type: string;
  summary?: string | null;
  metadataJson?: string | null;
  createdAtUtc: string;
}

export interface Notification {
  id: string;
  type: string;
  title: string;
  body: string;
  isRead: boolean;
  relatedEntityType?: string | null;
  relatedEntityId?: string | null;
  actorId?: string | null;
  createdAtUtc: string;
}

// ── Analytics ────────────────────────────────────────────────────────────────

export interface StatusCount {
  statusName: string;
  statusType: string;
  count: number;
}

export interface PriorityCount {
  priority: string;
  count: number;
}

export interface DailyCount {
  date: string;
  count: number;
}

export interface DashboardAnalytics {
  totalTasks: number;
  openTasks: number;
  completedTasks: number;
  overdueTasks: number;
  unassignedTasks: number;
  tasksByStatus: StatusCount[];
  tasksByPriority: PriorityCount[];
  tasksCreatedByDay: DailyCount[];
  tasksCompletedByDay: DailyCount[];
  averageCycleTimeDays: number;
}

export interface TeamVelocityPoint {
  weekStart: string;
  created: number;
  completed: number;
}

export interface AssigneeWorkload {
  assigneeId: string;
  openTasks: number;
  overdueTasks: number;
  completedTasks: number;
}

export interface ProjectProgress {
  projectId: string;
  projectKey: string;
  projectName: string;
  totalTasks: number;
  completedTasks: number;
  completionRate: number;
}

export interface CycleTimeBucket {
  label: string;
  count: number;
}

export interface CycleTimeAnalytics {
  sampleSize: number;
  averageCycleTimeDays: number;
  medianCycleTimeDays: number;
  p85CycleTimeDays: number;
  averageLeadTimeDays: number;
  histogram: CycleTimeBucket[];
}

export interface ThroughputPoint {
  periodStart: string;
  completedTasks: number;
  completedStoryPoints: number;
}

export interface BurndownPoint {
  date: string;
  remaining: number;
  idealRemaining: number;
  completedCumulative: number;
}

export interface SprintBurndown {
  sprintId: string;
  sprintName: string;
  startDate: string;
  endDate: string;
  unit: string;
  totalScope: number;
  points: BurndownPoint[];
}

// ── Search ───────────────────────────────────────────────────────────────────

export interface SearchResult {
  id: string;
  entityType: string;
  title: string;
  subtitle?: string | null;
  highlight?: string | null;
  rank: number;
  teamId?: string | null;
  projectId?: string | null;
}

// ── Reports ──────────────────────────────────────────────────────────────────

export interface ReportTaskRow {
  taskId: string;
  identifier: string;
  title: string;
  teamKey: string;
  projectKey?: string | null;
  status: string;
  priority: string;
  assigneeId?: string | null;
  dueDate?: string | null;
  createdAtUtc: string;
}

export interface TaskSummaryReport {
  generatedAtUtc: string;
  totalTasks: number;
  openTasks: number;
  completedTasks: number;
  overdueTasks: number;
  rows: ReportTaskRow[];
}

export interface OverdueTasksReport {
  generatedAtUtc: string;
  totalOverdue: number;
  rows: ReportTaskRow[];
}

export interface TeamPerformanceRow {
  teamId: string;
  teamKey: string;
  teamName: string;
  totalTasks: number;
  completedTasks: number;
  overdueTasks: number;
  completionRate: number;
  averageCycleTimeDays: number;
}

export interface TeamPerformanceReport {
  generatedAtUtc: string;
  teams: TeamPerformanceRow[];
}

// ── Files ────────────────────────────────────────────────────────────────────

export interface StoredFile {
  id: string;
  fileName: string;
  contentType: string;
  sizeBytes: number;
  publicUrl: string;
  entityType: string;
  entityId: string;
  uploadedById: string;
  createdAtUtc: string;
}

// ── Automation / audit ───────────────────────────────────────────────────────

export interface AutomationRule {
  id: string;
  name: string;
  triggerType: string;
  triggerConfigJson: string;
  actionType: string;
  actionConfigJson: string;
  isEnabled: boolean;
  createdAtUtc: string;
}

export interface AuditLogEntry {
  id: string;
  entityType: string;
  entityId: string;
  action: string;
  userId?: string | null;
  changesJson?: string | null;
  timestamp: string;
}

export interface AiChatMessage {
  role: 'user' | 'assistant';
  content: string;
}

export interface AiChatResponse {
  reply: string;
  toolsUsed: string[];
}
