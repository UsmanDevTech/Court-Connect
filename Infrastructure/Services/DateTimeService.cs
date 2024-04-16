using Application.Common.Interfaces;

namespace Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime NowUTC => DateTime.UtcNow;
    public string longDateFormat => "dd-MM-yyyy hh:mm tt";
    public string longDayDateFormat => "dddd, dd MMMM yyyy";
    public string longDayDateTimeFormat => "dd MMMM hh:mm tt";
    public string shortDateFormat => "dd-MM-yyyy";
    public string dateFormat => "yyyy-MM-dd";
    public string timeFormat => "HH:mm";
    public string dateTimeFormat => "hh:mm";
    public string dayFormat => "dd MMMM";

    public  string RelativeDate(DateTime date)
    {
        //var span = DateTime.FromFileTimeUtc(date.ToFileTime());
        var ts = new TimeSpan(DateTime.UtcNow.Ticks - date.Ticks);
        double delta = Math.Abs(ts.TotalSeconds);
        if (delta < 60)
        {
            return ts.Seconds == 1 ? "just now" : ts.Seconds + " seconds ago";
        }
        else if (delta < 60 * 2)
        {
            return "a minute ago";
        }
        else if (delta < 45 * 60)
        {
            return ts.Minutes + " minutes ago";
        }
        else if (delta < 90 * 60)
        {
            return "an hour ago";
        }
        else if (delta < 24 * 60 * 60)
        {
            return ts.Hours + " hours ago";
        }
        else if (delta < 48 * 60 * 60)
        {
            return "yesterday";
        }
        else if (delta < 30 * 24 * 60 * 60)
        {
            return ts.Days + " days ago";
        }
        else if (delta < 12 * 30 * 24 * 60 * 60)
        {
            int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
            return months <= 1 ? "one month ago" : months + " months ago";
        }
        int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
        return years <= 1 ? "one year ago" : years + " years ago";
    }
}
