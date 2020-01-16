using Cooking.WPF.Helpers;
using Microsoft.Win32;
using PhotoSauce.MagicScaler;
using System.IO;

namespace Cooking
{
    public class ImageService
    {
        private readonly ILocalization localization;

        public ImageService(ILocalization localization)
        {
            this.localization = localization;
        }

        public string? ImageSearch()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = localization.GetLocalizedString("RecipeIconSearch")
            };

            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                if (File.Exists(openFileDialog.FileName))
                {
                    var dir = Directory.CreateDirectory(Consts.ImageFolder);
                    var file = new FileInfo(openFileDialog.FileName);
                    var newFilePath = Path.Combine(dir.FullName, file.Name);

                    var inPath = openFileDialog.FileName;
                    var settings = new ProcessImageSettings { Width = 300 };

                    using var outStream = new FileStream(newFilePath, FileMode.CreateNew);
                    MagicImageProcessor.ProcessImage(inPath, outStream, settings);

                    return newFilePath;
                }
            }

            return null;
        }
    }
}
