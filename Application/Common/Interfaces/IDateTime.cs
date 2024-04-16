namespace Application.Common.Interfaces;

public interface IDateTime
{
    public DateTime NowUTC { get; }
    public string dateTimeFormat { get; }
    public string timeFormat { get; }
    public string dayFormat { get; }
    public string longDateFormat { get; }
    public string longDayDateFormat { get; }
    public string longDayDateTimeFormat { get; }
    public string dateFormat { get; }
    public string shortDateFormat { get; }
    public string RelativeDate(DateTime date);
   
}
