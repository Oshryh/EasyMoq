namespace EasyMoq.Tests.TestModelClasses.ForStaticClassesTests
{
    public class DatabaseSettings : IDatabaseSettings
    {
        
        public string this[string key] => GetSettingsValue(key);

        public string GetSettingsValue(string key)
        {
            switch (key)
            {
                case "Key1":
                    return "Value1";
                case "Key2":
                    return "Value2";
                case "Key3":
                    return "Value3";
                default:
                    return "Not found";
            }
        }

    }
}
