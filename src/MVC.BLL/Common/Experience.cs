namespace MVC.BLL.Common
{
    // D-03: years of experience is derived from PracticeStartYear and never stored
    public static class Experience
    {
        public static int Years(int practiceStartYear) =>
            Math.Max(0, DateTime.UtcNow.Year - practiceStartYear);
    }
}