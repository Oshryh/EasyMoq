namespace EasyMoq.Tests.TestModelClasses.ForStaticClassesTests
{
    public static class ConfigurationManager
    {
        private static IDatabaseSettings _settings;
        public static readonly object SettingsLock = new object();

        public static void WithDatabaseSettings(IDatabaseSettings settings = null)
        {
            if (_settings == null)
            {
                lock (SettingsLock)
                {
                    if (_settings == null)
                    {
                        _settings = settings ?? new DatabaseSettings();
                    }
                }
            }
        }

        public static IDatabaseSettings AppSettings
        {
            get
            {
                WithDatabaseSettings();

                return _settings;
            }
        }

        public static string GetSettingsValue(string key)
        {
            return _settings.GetSettingsValue(key);
        }
    }
}
