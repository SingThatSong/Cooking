using System.Collections.ObjectModel;
using Cooking.WPF.DTO;
using GongSolutions.Wpf.DragDrop;

namespace Cooking.WPF.ViewModels;

/// <summary>
/// Drag-and-drop logic for <see cref="RecipeViewModel"/>.
/// </summary>
public partial class RecipeViewModel : IDropTarget
{
    /// <inheritdoc/>
    public void DragOver(IDropInfo dropInfo)
    {
        dropInfo.Effects = System.Windows.DragDropEffects.Move;
        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
    }

    /// <inheritdoc/>
    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.TargetCollection != dropInfo.DragInfo.SourceCollection
         || dropInfo.Data == dropInfo.TargetItem
         || dropInfo.Data is not RecipeIngredientEdit ingredient
         || dropInfo.TargetItem is not RecipeIngredientEdit targetIngredient
         || dropInfo.TargetCollection is not ObservableCollection<RecipeIngredientEdit> targetCollection)
        {
            return;
        }

        targetCollection.Remove(ingredient);

        int targetIndex = targetCollection.IndexOf(targetIngredient);
        if (dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.AfterTargetItem))
        {
            targetCollection.Insert(targetIndex + 1, ingredient);
        }
        else if (dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.BeforeTargetItem))
        {
            targetCollection.Insert(targetIndex, ingredient);
        }

        for (int i = 0; i < targetCollection.Count; i++)
        {
            targetCollection[i].Order = i;
        }
    }
}
