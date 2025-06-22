// ⠀
// StorageServiceTests.cs
// TiContent.Foundation.Services.UnitTests
// 
// Created by the_timick on 18.06.2025.
// ⠀

using Microsoft.Extensions.Logging;
using Moq;
using TiContent.Foundation.Entities.Storage;

namespace TiContent.Foundation.Services.UnitTests;

[TestFixture]
public class StorageServiceTests
{
    private readonly string _file = Path.Combine(AppContext.BaseDirectory, "TiContent.storage.test.json");

    [SetUp]
    public void SetUp()
    {
        if (File.Exists(_file))
            File.Delete(_file);
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_file))
            File.Delete(_file);
    }

    [Test]
    public void ObtainTest()
    {
        // Give
        var expected = new StorageEntity();

        var loggerMock = new Mock<ILogger<StorageService>>();
        var storage = new StorageService(_file, loggerMock.Object);

        // When
        var cached = storage.Obtain();

        // Then
        Assert.Multiple(() =>
            {
                Assert.That(cached, Is.EqualTo(expected));
                Assert.That(storage.Cached, Is.EqualTo(cached));
            }
        );
    }

    [Test]
    public void SaveTest()
    {
        // Give
        var expected = new StorageEntity { Keys = new StorageEntity.KeysEntity { TMDBApiKey = "12345" } };

        var loggerMock = new Mock<ILogger<StorageService>>();
        var storage = new StorageService(_file, loggerMock.Object);

        // When
        var cached = storage.Obtain();
        cached.Keys.TMDBApiKey = "12345";
        storage.Save(false);

        // Then
        storage = new StorageService(_file, loggerMock.Object);
        cached = storage.Obtain();

        Assert.That(cached, Is.EqualTo(expected));
    }
}