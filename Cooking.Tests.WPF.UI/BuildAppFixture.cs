using System;
using System.Diagnostics;
using System.IO;

namespace Cooking.Tests.UITests;

public class BuildAppFixture : IDisposable
{
    public string OutputDirectory { get; }

    public BuildAppFixture()
    {
        OutputDirectory = $@"{Directory.GetCurrentDirectory()}\BinariesForUITests";
        string path = $@"/C dotnet build ..\..\..\..\Cooking.WPF\Cooking.WPF.csproj -o {OutputDirectory}";
        Process.Start("cmd.exe", path).WaitForExit();
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
            Directory.Delete(OutputDirectory, recursive: true);
        }
    }
}
