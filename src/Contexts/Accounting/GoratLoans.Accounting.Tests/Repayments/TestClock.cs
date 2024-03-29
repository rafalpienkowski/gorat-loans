using GoratLoans.Domain;

namespace GoratLoans.Accounting.Tests.Repayments;

internal class TestClock : IClock
{
    private DateTimeOffset _clock;

    public DateTimeOffset Now => _clock;

    public TestClock()
    {
        _clock = DateTimeOffset.UtcNow;
    }

    public void AddDays(int days)
    {
        _clock = _clock.AddDays(days).AddMinutes(1);
    }
}