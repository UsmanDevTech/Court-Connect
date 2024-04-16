using TimeZoneConverter;

namespace Application.Common.Extensions;


public static class DateTimeExtension
{
    public static DateTime UtcToLocal(this DateTime source, TimeZoneInfo localTimeZone)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(source, localTimeZone);
    }

    public static DateTime LocalToUtc(this DateTime source, string localTimeZone)
    {
        TimeZoneInfo tzInfo = TZConvert.GetTimeZoneInfo(localTimeZone);
        return TimeZoneInfo.ConvertTimeToUtc(source, tzInfo);
    }

    public static DateTime UtcToLocalTime(this DateTime source, string localTimeZone)
    {

        TimeZoneInfo meTimeZone = TZConvert.GetTimeZoneInfo(localTimeZone);
        return TimeZoneInfo.ConvertTimeFromUtc(source, meTimeZone);
    }

    public static DateTimeOffset local(this DateTimeOffset source, string localTimeZone)
    {
        TimeZoneInfo tzInfo = TZConvert.GetTimeZoneInfo(localTimeZone);
        DateTimeOffset convertedTime = TimeZoneInfo.ConvertTime(source, tzInfo);

        return convertedTime;
    }


}
