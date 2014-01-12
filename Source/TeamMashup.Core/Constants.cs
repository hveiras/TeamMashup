
namespace TeamMashup.Core
{
    public static class Constants
    {
        public const long DefaultProfilePictureId = 0;
        public const long InvalidId = 0;

        public static bool IsValidId(this long id)
        {
            return id > InvalidId;
        }

        public static bool IsValidId(this int id)
        {
            return id > InvalidId;
        }

        public const string LocalCountryName = "Argentina";
    }

    public static class LogConstants
    {
        public const string LoginSuccessful = "user {0} logged successfuly";
        public const string LoginFailed = "user {0} failed to login";
    }
}