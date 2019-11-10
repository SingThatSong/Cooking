using Cooking.DTO;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;

namespace Cooking.Pages.Recepies
{
    public partial class RecipeViewModel
    {
        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = System.Windows.DragDropEffects.Move;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.TargetCollection != dropInfo.DragInfo.SourceCollection) return;
            if (dropInfo.Data == dropInfo.TargetItem) return;
            if (!(dropInfo.Data is RecipeIngredientMain ingredient)) return;
            if (!(dropInfo.TargetItem is RecipeIngredientMain targetIngredient)) return;
            if (!(dropInfo.TargetCollection is ObservableCollection<RecipeIngredientMain> targetCollection)) return;

            var oldIndex = targetCollection.IndexOf(ingredient);
            var targetIndex = targetCollection.IndexOf(targetIngredient);

            // If we'll be inserting item before it's current position, it's previous position will change +1
            oldIndex = targetIndex < oldIndex ? oldIndex + 1 : oldIndex;

            if (dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.AfterTargetItem))
            {
                targetCollection.Insert(targetIndex + 1, ingredient);
            }
            else if (dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.BeforeTargetItem))
            {
                targetCollection.Insert(targetIndex, ingredient);
            }

            targetCollection.RemoveAt(oldIndex);

            for (int i = 0; i < targetCollection.Count; i++)
            {
                targetCollection[i].Order = i;
            }
        }
    }
}