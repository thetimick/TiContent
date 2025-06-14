// ⠀
// PluralHelperTests.cs
// TiContent.Foundation.Components.UnitTests
// 
// Created by the_timick on 15.06.2025.
// ⠀

using TiContent.Foundation.Components.Helpers;

namespace TiContent.Foundation.Components.UnitTests.Helpers;

[TestFixture]
public class PluralHelperTests
{
    [TestCase(0, "яблоко", "яблока", "яблок", true, "0 яблок")]
    [TestCase(1, "яблоко", "яблока", "яблок", true, "1 яблоко")]
    [TestCase(2, "яблоко", "яблока", "яблок", true, "2 яблока")]
    [TestCase(0, "яблоко", "яблока", "яблок", false, "яблок")]
    [TestCase(1, "яблоко", "яблока", "яблок", false, "яблоко")]
    [TestCase(2, "яблоко", "яблока", "яблок", false, "яблока")]
    [TestCase(5, "яблоко", "яблока", "яблок", true, "5 яблок")]
    [TestCase(11, "яблоко", "яблока", "яблок", true, "11 яблок")]
    [TestCase(21, "яблоко", "яблока", "яблок", true, "21 яблоко")]
    [TestCase(22, "яблоко", "яблока", "яблок", true, "22 яблока")]
    [TestCase(25, "яблоко", "яблока", "яблок", true, "25 яблок")]
    [TestCase(-1, "яблоко", "яблока", "яблок", true, "-1 яблоко")]
    public void PluralizeTests(
        int number,
        string form1,
        string form2,
        string form5,
        bool withValue,
        string expected
    )
    {
        var result = PluralHelper.Pluralize(number, form1, form2, form5, withValue);
        Assert.That(result, Is.EqualTo(expected));
    }
}