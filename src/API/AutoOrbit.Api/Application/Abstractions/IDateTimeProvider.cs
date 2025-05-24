namespace AutoOrbit.Api.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

