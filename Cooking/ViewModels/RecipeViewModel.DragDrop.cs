using Cooking.WPF.DTO;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;

namespace Cooking.WPF.Views
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
#pragma warning disable IDE0011 // Добавить фигурные скобки
            if (dropInfo.TargetCollection != dropInfo.DragInfo.SourceCollection) return;
            if (dropInfo.Data == dropInfo.TargetItem) return;
            if (!(dropInfo.Data is RecipeIngredientEdit ingredient)) return;
            if (!(dropInfo.TargetItem is RecipeIngredientEdit targetIngredient)) return;
            if (!(dropInfo.TargetCollection is ObservableCollection<RecipeIngredientEdit> targetCollection)) return;
#pragma warning restore IDE0011

            int oldIndex = targetCollection.IndexOf(ingredient);
            int targetIndex = targetCollection.IndexOf(targetIngredient);

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