// ⠀
// CollectionExtensionsTests.cs
// TiContent.Foundation.Components
//
// Created by the_timick on 13.06.2025.
// 

using System.Collections.ObjectModel;
using TiContent.Foundation.Components.Extensions;

namespace TiContent.Foundation.Components.UnitTests.Extensions;

[TestFixture]
public class CollectionExtensionsTests
{
    [Test]
    public void IsEmptyTest()
    {
        var collection = new List<string>();
        Assert.That(collection.IsEmpty(), Is.EqualTo(true));

        collection.Add("string");
        Assert.That(collection.IsEmpty(), Is.EqualTo(false));
    }

    [Test]
    public void ToObservableTest()
    {
        var observable = new List<string>()
            .ToObservable();
        Assert.That(observable.GetType(), Is.EqualTo(typeof(ObservableCollection<string>)));
    }

    [Test]
    public void GetSaveTest()
    {
        var collection = new List<string> { "string" };
        Assert.Multiple(() =>
            {
                Assert.That(collection.GetSafe(0), Is.EqualTo("string"));
                Assert.That(collection.GetSafe(1), Is.Null);
            }
        );
    }
}