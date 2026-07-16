using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Files.Dtos;

namespace WorkPulse.Application.Files.Queries;

public sealed record GetFileMetadataQuery(Guid FileId) : IRequest<Result<StoredFileDto>>, IBaseRequest, IQuery;
