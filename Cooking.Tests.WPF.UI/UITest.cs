using System;
using System.IO;
using Xunit;

namespace Cooking.Tests.UITests;

[Collection("UI Test")]
public class UITest : IDisposable
{
    protected BuildAppFixture AppFixture { get; }

    public UITest(BuildAppFixture appFixture)
    {
        AppFixture = appFixture;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            File.Delete(@$"{AppFixture.OutputDirectory}\cooking.db");
        }
    }
}
