namespace TeamMashup.Core.Domain
{
    public static class DatabaseContextExtensions
    {
        public static BackupRequest AddBackupRequest(this IDatabaseContext context, BackupRequest request)
        {
            return context.BackupRequests.Add(request);
        }

        public static Country AddCountry(this IDatabaseContext context, Country country)
        {
            return context.Countries.Add(country);
        }

        public static Language AddLanguage(this IDatabaseContext context, Language language)
        {
            return context.Languages.Add(language);
        }

        public static Survey AddSurvey(this IDatabaseContext context, Survey survey)
        {
            return context.Surveys.Add(survey);
        }
    }
}
