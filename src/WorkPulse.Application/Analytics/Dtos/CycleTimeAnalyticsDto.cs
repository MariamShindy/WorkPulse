
namespace WorkPulse.Application.Analytics.Dtos;

public sealed record CycleTimeAnalyticsDto(int SampleSize, double AverageCycleTimeDays, double MedianCycleTimeDays, double P85CycleTimeDays, double AverageLeadTimeDays, IReadOnlyList<CycleTimeBucketDto> Histogram);
