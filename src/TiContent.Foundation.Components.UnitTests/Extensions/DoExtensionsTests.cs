// ⠀
// DoExtensionsTests.cs
// TiContent.Foundation.Components.UnitTests
// 
// Created by the_timick on 15.06.2025.
// ⠀

using TiContent.Foundation.Components.Extensions;

namespace TiContent.Foundation.Components.UnitTests.Extensions;

[TestFixture]
public class DoExtensionsTests
{
    [Test]
    public void DoTest()
    {
        var collection = new List<string> { "string_1" };
        collection.Do(list => list.Add("string_2"));
        Assert.That(collection, Does.Contain("string_2"));
    }
}