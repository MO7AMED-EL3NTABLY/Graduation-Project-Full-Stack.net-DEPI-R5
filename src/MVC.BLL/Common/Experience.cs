/*——- Omar Hussein ——-*/
namespace MVC.BLL.Common
{
    // D-03: سنين الخبرة قيمة محسوبة من PracticeStartYear ومش بتتخزن
    public static class Experience
    {
        public static int Years(int practiceStartYear) =>
            Math.Max(0, DateTime.UtcNow.Year - practiceStartYear);
    }
}
/*————*/
