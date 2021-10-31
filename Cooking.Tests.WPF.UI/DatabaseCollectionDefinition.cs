using Xunit;

namespace Cooking.Tests.UITests;

[CollectionDefinition("UI Test")]
public class DatabaseCollectionDefinition : ICollectionFixture<BuildAppFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
