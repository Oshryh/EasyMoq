using Moq;

namespace EasyMoq
{
    public static class StaticMockOf<T> where T : class
    {
        public static Mock<T> Instance { get; } = new Mock<T>();
    }
}
