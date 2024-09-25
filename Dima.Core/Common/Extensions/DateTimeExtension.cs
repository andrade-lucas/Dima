namespace Dima.Core.Common.Extensions;

public static class DateTimeExtension
{
    public static DateTime StartOfMonth(this DateTime date, int? year = null, int? month = null)
        => new(year ?? date.Year, month ?? date.Month, 1);

    public static DateTime EndOfMonth(this DateTime date, int? year = null, int? month = null)
        => new DateTime(year ?? date.Year, month ?? date.Month, 1)
            .AddMonths(1)
            .AddDays(-1);
}
