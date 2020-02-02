using Cooking.WPF.Services;
using Microsoft.Win32;
using PhotoSauce.MagicScaler;
using System;
using System.IO;

namespace Cooking
{
    /// <summary>
    /// Service for working with images.
    /// </summary>
    public class ImageService
    {
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageService"/> class.
        /// </summary>
        /// <param name="localization">Localization provider dependency.</param>
        public ImageService(ILocalization localization)
        {
            this.localization = localization;
        }

        /// <summary>
        /// Select an image, resize it and save to predefined folder.
        /// </summary>
        /// <returns>Saved file path.</returns>
        public string? UseImage()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Title = localization.GetLocalizedString("RecipeIconSearch")
            };

            bool? dialogResult = openFileDialog.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                if (File.Exists(openFileDialog.FileName))
                {
                    DirectoryInfo dir = Directory.CreateDirectory(Consts.ImageFolder);
                    var file = new FileInfo(openFileDialog.FileName);
                    string newFilePath = Path.Combine(dir.FullName, $"{Guid.NewGuid()}{file.Extension}");
                    MinifyImage(source: openFileDialog.FileName, destination: newFilePath);
                    return newFilePath;
                }
            }

            return null;
        }

        private void MinifyImage(string source, string destination)
        {
            var settings = new ProcessImageSettings { Width = Consts.ImageWidth };

            using var outStream = new FileStream(destination, FileMode.CreateNew);
            MagicImageProcessor.ProcessImage(source, outStream, settings);
        }
    }
}
