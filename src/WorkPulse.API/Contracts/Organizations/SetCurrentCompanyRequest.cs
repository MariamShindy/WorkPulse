using System;

namespace WorkPulse.API.Contracts.Organizations;

public sealed record SetCurrentCompanyRequest(Guid CompanyId);
