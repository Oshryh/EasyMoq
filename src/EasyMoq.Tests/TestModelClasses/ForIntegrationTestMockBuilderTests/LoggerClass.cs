namespace EasyMoq.Tests.TestModelClasses.ForIntegrationTestMockBuilderTests
{
    public class LoggerClass : ILoggerClass
    {
        public string LogMessage()
        {
            return "Logged a message";
        }
    }
}