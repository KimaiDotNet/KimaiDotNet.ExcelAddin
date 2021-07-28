namespace MarkZither.KimaiDotNet.ExcelAddin.Services
{
    public class CalendarServiceBase
    {
        public CalendarServiceBase(string url, string username, string password)
        {
            RootUrl = url;
            Username = username;
            Password = password;
        }
        public string Username { get; }
        public string Password { get; }
        public string RootUrl { get; }
    }
}