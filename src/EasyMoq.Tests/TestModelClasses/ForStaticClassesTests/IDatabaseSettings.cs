namespace EasyMoq.Tests.TestModelClasses.ForStaticClassesTests
{
    public interface IDatabaseSettings
    {
        string this[string key] { get; }
        string GetSettingsValue(string key);
    }
}