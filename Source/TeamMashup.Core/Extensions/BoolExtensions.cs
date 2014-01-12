
namespace TeamMashup.Core
{
    public static class BoolExtensions
    {
        public static string AsYesNo(this bool value)
        {
            return value ? "Yes" : "No";
        }
    }
}
