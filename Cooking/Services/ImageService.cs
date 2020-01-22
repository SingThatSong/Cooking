using Cooking.WPF.Helpers;
using Microsoft.Win32;
using PhotoSauce.MagicScaler;
using System.IO;

namespace Cooking
{
    public class ImageService
    {
        private readonly ILocalization localization;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageService"/> class.
        /// </summary>
        /// <param name="localization"></param>
        public ImageService(ILocalization localization)
        {
            this.localization = localization;
        }

        public string? ImageSearch()
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
                    string newFilePath = Path.Combine(dir.FullName, file.Name);

                    string inPath = openFileDialog.FileName;
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
