using System;
using System.Linq;
using System.Threading.Tasks;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using FluentAssertions;
using Xunit;

namespace Cooking.Tests.UITests;

public class TagTest : UITest
{
    public TagTest(BuildAppFixture appFixture) : base(appFixture)
    {
    }

    [Fact]
    public async Task CreateTag_CreatesTag()
    {
        using var app = FlaUI.Core.Application.Launch(@$"{AppFixture.OutputDirectory}\Cooking.WPF.exe");
        using var automation = new UIA3Automation();
        Window? window = app.GetMainWindow(automation, TimeSpan.FromSeconds(15));
        await CreateTag(window);

        AutomationElement[]? tagsAfterAdd = window.FindAllDescendants(cf => cf.ByAutomationId("TagTextBlock"));

        app.Kill();
        tagsAfterAdd.Length.Should().Be(2);
    }

    [Fact]
    public async Task DeleteTag_DeletesTag()
    {
        using var app = FlaUI.Core.Application.Launch(@$"{AppFixture.OutputDirectory}\Cooking.WPF.exe");
        using var automation = new UIA3Automation();
        Window? window = app.GetMainWindow(automation, TimeSpan.FromSeconds(15));

        await CreateTag(window);

        // Click delete button on last tag
        window.FindAllDescendants(x => x.ByAutomationId("TagDeleteButton")).Last().AsButton().Invoke();

        await WaitForDialogOpen();

        // When menu shown, press left (by default it's focused on Decline button) and Enter to submit
        Keyboard.Press(VirtualKeyShort.LEFT);
        await Task.Delay(50);
        Keyboard.Press(VirtualKeyShort.ENTER);

        await WaitForDialogClose();
        // Wait for grid update
        await Task.Delay(300);

        AutomationElement[]? tagsAfter = window.FindAllDescendants(cf => cf.ByAutomationId("TagTextBlock"));
        app.Kill();
        tagsAfter.Length.Should().Be(1);
    }

    private static async Task CreateTag(Window window)
    {
        AutomationElement? menuItems = window.FindFirstChild("ButtonsListView");

        menuItems.FindAllChildren()[4].AsListBoxItem().Select();

        window.FindFirstDescendant(cf => cf.ByAutomationId("AddTagButton")).AsButton().Invoke();

        await WaitForDialogOpen();

        window.FindFirstDescendant(cf => cf.ByAutomationId("TagNameTextBox")).AsTextBox().Text = Guid.NewGuid().ToString();
        window.FindFirstDescendant(cf => cf.ByAutomationId("TagTypeComboBox")).AsComboBox().Select("Источник");
        window.FindFirstDescendant(cf => cf.ByAutomationId("OkButton")).AsButton().Invoke();

        await WaitForDialogClose();
    }

    private static async Task WaitForDialogOpen()
    {
        // Wait for dialog open animation
        await Task.Delay(500);
    }

    private static async Task WaitForDialogClose()
    {
        // Wait for dialog close animation
        await Task.Delay(300);
    }
}
