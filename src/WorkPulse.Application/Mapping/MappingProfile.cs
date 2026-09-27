using AutoMapper;
using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Mapping;

public sealed class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<TaskItem, TaskItemDto>().ForMember((TaskItemDto d) => d.TeamKey, delegate(IMemberConfigurationExpression<TaskItem, TaskItemDto, string> opt)
		{
			opt.Ignore();
		}).ForMember((TaskItemDto d) => d.Identifier, delegate(IMemberConfigurationExpression<TaskItem, TaskItemDto, string> opt)
		{
			opt.Ignore();
		}).ForMember((TaskItemDto d) => d.ProjectKey, delegate(IMemberConfigurationExpression<TaskItem, TaskItemDto, string?> opt)
		{
			opt.Ignore();
		})
			.ForMember((TaskItemDto d) => d.WorkflowStateName, delegate(IMemberConfigurationExpression<TaskItem, TaskItemDto, string> opt)
			{
				opt.Ignore();
			})
			.ForMember((TaskItemDto d) => d.WorkflowStateType, delegate(IMemberConfigurationExpression<TaskItem, TaskItemDto, string> opt)
			{
				opt.Ignore();
			})
			.ForMember((TaskItemDto d) => d.Priority, delegate(IMemberConfigurationExpression<TaskItem, TaskItemDto, string> opt)
			{
				opt.MapFrom((TaskItem s) => s.Priority.ToString());
			});
	}
}
