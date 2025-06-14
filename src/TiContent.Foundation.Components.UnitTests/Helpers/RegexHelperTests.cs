// ⠀
// RegexHelperTests.cs
// TiContent.Foundation.Components.UnitTests
// 
// Created by the_timick on 15.06.2025.
// ⠀

using TiContent.Foundation.Components.Helpers;

namespace TiContent.Foundation.Components.UnitTests.Helpers;

[TestFixture]
public class RegexHelperTests
{
    [
        TestCase(
            "[DL] God of War: Ragnarök (Ragnarok) [P] [RUS + ENG + 22 / RUS + ENG + 10] (2024, TPS) (1.1.1) [Portable]",
            "dlgodofwarragnarkragnarokpruseng22ruseng102024tps111portable"
        )
    ]
    [
        TestCase(
            "Игры-детям. Русалочка [P] [RUS + ENG + 2 / RUS + ENG + 2] (1999) [3—8]",
            "игрыдетямрусалочкаpruseng2ruseng2199938"
        )
    ]
    public void CleanTests(string rawValue, string expectedValue)
    {
        var str = RegexHelper.Clean().Replace(rawValue.Trim().ToLowerInvariant(), "");
        Assert.That(str, Is.EqualTo(expectedValue));
    }
}