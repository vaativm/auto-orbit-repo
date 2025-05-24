using AutoOrbit.Api.Application.Abstractions;

namespace AutoOrbit.Api.Infrastructure.Time;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
