using Cooking.Data.Model;
using PropertyChanged;
using System.IO;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// Shallow recipe DTO for showing in lists.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class RecipeListViewDto : Entity
    {
        /// <summary>
        /// Gets or sets recipe name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets recipe calorie type.
        /// </summary>
        public CalorieType CalorieType { get; set; }

        /// <summary>
        /// Gets or sets image path.
        /// </summary>
        [AlsoNotifyFor(nameof(FullPath))]
        public string? ImagePath { get; set; }

        /// <summary>
        /// Gets absolute path to recipe.
        /// </summary>
        public string? FullPath => ImagePath != null && File.Exists(Path.GetFullPath(ImagePath))
                                ? Path.GetFullPath(ImagePath)
                                : null;

        /// <summary>
        /// Gets or sets recipe rating.
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Gets or sets count of days since recipe was last cooked. Not edited by user.
        /// </summary>
        public int LastCooked { get; set; }
    }
}
