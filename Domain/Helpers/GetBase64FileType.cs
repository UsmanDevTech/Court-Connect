namespace Domain.Helpers;
public static class StringHelper
{
    public static string GetBase64FileType(this string base64String)
    {
        var data = base64String.Substring(0, 5);

        switch (data.ToUpper())
        {
            case "AAAAG":
                return ".m4a";
            case "SUQZA":
                return ".mp3";
            case "IVBOR":
                return ".png";
            case "/9J/4":
                return ".jpg";
            case "AAAAF":
                return ".mp4";
            case "AAAAI":
                return ".mp4";
            case "JVBER":
                return ".pdf";
            case "AAABA":
                return ".ico";
            case "UMFYI":
                return ".rar";
            case "E1XYD":
                return ".rtf";
            case "U1PKC":
                return ".txt";
            case "MQOWM":
            case "77U/M":
                return ".srt";
            default:
                return string.Empty;
        }
    }
}
