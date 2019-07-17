using Moq;

namespace EasyMoq
{
    public static class StaticMockOf<T> where T : class
    {
        private static Mock<T> _settingsMock;
        private static readonly object _settingsLock = new object();

        public static Mock<T> Get()
        {
            if (_settingsMock == null)
            {
                lock (_settingsLock)
                {
                    if (_settingsMock == null)
                    {
                        _settingsMock = new Mock<T>();
                    }
                }
            }

            return _settingsMock;
        }
    }
}
