using Cooking.Data.Model;
using PropertyChanged;
using System.Threading;
using Validar;

namespace Cooking.WPF.DTO
{
    [InjectValidation]
    [AddINotifyPropertyChangedInterface]
    public class RecipeIngredientEdit : Entity
    {
        public RecipeIngredientEdit()
        {
            Culture = Thread.CurrentThread.CurrentUICulture.Name;
        }

        public IngredientEdit? Ingredient { get; set; }

        // Validation of Amount and MeasureUnit depends on each other, so we have to notify interface about changes
        [AlsoNotifyFor(nameof(MeasureUnit))]
        public double? Amount { get; set; }
        [AlsoNotifyFor(nameof(Amount))]
        public MeasureUnit? MeasureUnit { get; set; }

        public int Order { get; set; }
    }
}