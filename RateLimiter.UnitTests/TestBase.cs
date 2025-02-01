using System.Diagnostics.CodeAnalysis;

namespace RateLimiter.UnitTests;

[ExcludeFromCodeCoverage]
public class TestBase
{
    /// <summary>
    /// A method which generates invalid string test data.
    /// </summary>
    public static IEnumerable<object?[]> InvalidStringTestData()
    {
        return new List<object?[]>
        {
            new object[] { string.Empty },
            new object[] { " " },
            new object[] { "\t" },
            new object[] { "\n" },
            new object[] { "\r" },
            new object[] { "\r\n" },
            new object[] { " \t\n\r" },
            new object?[] { null },
        };
    }
}