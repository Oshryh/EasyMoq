namespace EasyMoq
{
    public enum MockStrategy
    {
        /// <summary>
        /// Mock everything possible
        /// </summary>
        UnitTest,

        /// <summary>
        /// Mock only what's requested to be mocked, and use the container for the rest
        /// </summary>
        Integration
    }
}
