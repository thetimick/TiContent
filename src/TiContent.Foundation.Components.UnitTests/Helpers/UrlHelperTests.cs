// ⠀
// UrlHelperTests.cs
// TiContent.Foundation.Components.UnitTests
// 
// Created by the_timick on 15.06.2025.
// ⠀

using TiContent.Foundation.Components.Helpers;

namespace TiContent.Foundation.Components.UnitTests.Helpers;

[TestFixture]
public class UrlHelperTests
{
    [TestCase("https://example.com", new[] { "api", "v1" }, "https://example.com/api/v1")]
    [TestCase("https://example.com/", new[] { "/api/", "/v1/" }, "https://example.com/api/v1")]
    [TestCase("https://example.com/path", new[] { "sub", "item" }, "https://example.com/path/sub/item")]
    [TestCase("https://example.com/path/", new[] { "", "sub", "", "item/" }, "https://example.com/path/sub/item")]
    [TestCase("https://example.com/path?token=abc", new[] { "api", "v1" }, "https://example.com/path/api/v1?token=abc")]
    [TestCase("https://example.com/path", new string[0], "https://example.com/path")]
    [TestCase("https://example.com", new[] { "", null, "api", "", "v1" }, "https://example.com/api/v1")]
    public void CombineTests(string baseUrl, string[] segments, string expected)
    {
        var result = UrlHelper.Combine(baseUrl, segments);
        Assert.That(result, Is.EqualTo(expected));
    }
}