using Cooking.Data.Model;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Validar;

namespace Cooking.WPF.DTO
{
    /// <summary>
    /// Dto for recipe editing and view.
    /// </summary>
    [InjectValidation]
    public class RecipeEdit : EntityNotify
    {
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
        /// Gets or sets ingredient groups in recipe.
        /// </summary>
        public ObservableCollection<IngredientGroupEdit>? IngredientGroups { get; set; }

        /// <summary>
        /// Gets or sets ingredients in recipe.
        /// </summary>
        public ObservableCollection<RecipeIngredientEdit>? Ingredients { get; set; }

        /// <summary>
        /// Gets or sets ingredient tags in recipe.
        /// </summary>
        public ObservableCollection<TagEdit>? Tags { get; set; }

        /// <summary>
        /// Gets or sets garnishes for recipe.
        /// </summary>
        public ObservableCollection<GarnishEdit> Garnishes { get; set; } = new ObservableCollection<GarnishEdit>();

        /// <summary>
        /// Gets or sets count of days since recipe was last cooked. Not edited by user.
        /// </summary>
        public int LastCooked { get; set; }

        /// <summary>
        /// Gets or sets source of the recipe: website, granny's cookbook, etc.
        /// </summary>
        public Uri? SourceUrl { get; set; }

        /// <summary>
        /// Gets or sets recipe Description. Or recipe itself.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets how many portions can you cook with single recipe.
        /// </summary>
        public int PortionsCount { get; set; }

        /// <summary>
        /// Gets or sets recipe difficulty.
        /// </summary>
        public int? Difficulty { get; set; }

        /// <summary>
        /// Gets or sets recipe rating.
        /// </summary>
        public int? Rating { get; set; }

        /// <summary>
        /// Gets or sets recipe name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets recipe calorie type.
        /// </summary>
        public CalorieType CalorieType { get; set; }
    }
}
