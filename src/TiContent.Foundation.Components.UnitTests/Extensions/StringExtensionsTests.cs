// ⠀
// StringExtensionsTests.cs
// TiContent.Foundation.Components.UnitTests
// 
// Created by the_timick on 15.06.2025.
// ⠀

using TiContent.Foundation.Components.Extensions;

namespace TiContent.Foundation.Components.UnitTests.Extensions;

[TestFixture]
public class StringExtensionsTests
{
    [Test]
    public void IsNullOrEmptyTest()
    {
        string? str = null;
        Assert.That(str.IsNullOrEmpty(), Is.True);

        str = "";
        Assert.That(str.IsNullOrEmpty(), Is.True);

        str = "string";
        Assert.That(str.IsNullOrEmpty(), Is.False);
    }

    [Test]
    public void IsNullOrWhiteSpaceTest()
    {
        string? str = null;
        Assert.That(str.IsNullOrWhiteSpace(), Is.True);

        str = "   ";
        Assert.That(str.IsNullOrWhiteSpace(), Is.True);

        str = "string";
        Assert.That(str.IsNullOrWhiteSpace(), Is.False);
    }

    [Test]
    public void OrTest()
    {
        string? str = null;
        Assert.That(str.Or("or"), Is.EqualTo("or"));

        str = "string";
        Assert.That(str.Or("or"), Is.EqualTo("string"));
    }

    [Test]
    public void OrEmptyTest()
    {
        string? str = null;
        Assert.That(str.OrEmpty(), Is.EqualTo(""));

        str = "string";
        Assert.That(str.OrEmpty(), Is.EqualTo("string"));
    }
}