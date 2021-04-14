using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using FlaUI.UIA3;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cooking.Tests.UITests
{
    public class Test
    {
        [Fact]
        public async Task CreateRecipe_CreatesRecipe()
        {
            using var app = FlaUI.Core.Application.Launch(@"D:\Repos\Cooking\Cooking.WPF\bin\x64\Debug\net6.0-windows\win10-x64\Cooking.WPF.exe");
            using var automation = new UIA3Automation();
            Window? window = app.GetMainWindow(automation, TimeSpan.FromSeconds(15));
            await Task.Delay(1000);

            AutomationElement? menuItems = window.FindFirstChild("ButtonsListView");

            menuItems.FindAllChildren()[4].AsListBoxItem().Select();

            window.FindFirstDescendant(cf => cf.ByAutomationId("AddTagButton")).AsButton().Invoke();

            AutomationElement[]? tags = window.FindAllDescendants(cf => cf.ByAutomationId("TagTextBlock"));

            // ждём отрисовки анимации появления окна
            await Task.Delay(500);
            window.FindFirstDescendant(cf => cf.ByAutomationId("TagNameTextBox")).AsTextBox().Text = Guid.NewGuid().ToString();
            window.FindFirstDescendant(cf => cf.ByAutomationId("TagTypeComboBox")).AsComboBox().Select("Источник");
            window.FindFirstDescendant(cf => cf.ByAutomationId("OkButton")).AsButton().Invoke();

            // ждём отрисовки анимации появления окна
            await Task.Delay(500);
            AutomationElement[]? tagsAfterAdd = window.FindAllDescendants(cf => cf.ByAutomationId("TagTextBlock"));

            tagsAfterAdd.Length.Should().Be(tags.Length + 1);

            app.Kill();
        }
    }
}
