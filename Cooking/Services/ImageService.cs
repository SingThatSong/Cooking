using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Cooking
{
    public class ImageService
    {
        public string? ImageSearch()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Поиск изображения для рецепта"
            };

            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                if (File.Exists(openFileDialog.FileName))
                {
                    var dir = Directory.CreateDirectory(Consts.ImageFolder);
                    var file = new FileInfo(openFileDialog.FileName);
                    var newFilePath = Path.Combine(dir.FullName, file.Name);
                    if (File.Exists(newFilePath))
                    {
                        File.Delete(newFilePath);
                    }

                    using var img = new Bitmap(openFileDialog.FileName);
                    using var result = ResizeImage(img, 300);
                    result.Save(newFilePath);

                    return $@"{Consts.ImageFolder}/{file.Name}";
                }
            }

            return null;
        }

        /// <summary>
        /// https://stackoverflow.com/a/24199315
        /// </summary>
        public Bitmap ResizeImage(Image image, int height)
        {
            var newWidth = image.Width * height / image.Height;
            var destRect = new Rectangle(0, 0, newWidth, height);
            var destImage = new Bitmap(newWidth, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.Low;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using var wrapMode = new ImageAttributes();
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }

            return destImage;
        }
    }
}
