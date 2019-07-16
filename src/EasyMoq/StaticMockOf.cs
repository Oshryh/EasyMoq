﻿using Moq;

namespace EasyMoq
{
    public static class StaticMockOf<T> where T : class
    {
        private static Mock<T> _settingsMock;
        public static readonly object SettingsLock = new object();

        public static Mock<T> Get()
        {
            if (_settingsMock == null)
            {
                lock (SettingsLock)
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
