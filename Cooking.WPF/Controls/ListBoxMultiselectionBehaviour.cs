using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Cooking.WPF.Controls;

/// <summary>
/// Behaviour for binding SelectedItems.
/// N.B. ListBox must have SelectionMode="Multiple".
/// </summary>
public class ListBoxMultiselectionBehaviour : Behavior<ListBox>
{
    /// <summary>
    /// DependencyProperty for <see cref="SelectedItems"/>.
    /// </summary>
    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
                                                                            nameof(SelectedItems),
                                                                            typeof(INotifyCollectionChanged),
                                                                            typeof(ListBoxMultiselectionBehaviour),
                                                                            new PropertyMetadata(propertyChangedCallback: OnSelectedItemsPropertyChanged));

    /// <summary>
    /// Gets or sets getter/Setter for DependencyProperty, bound to the DataContext's SelectedItems ObservableCollection.
    /// </summary>
    public INotifyCollectionChanged? SelectedItems
    {
        get => (INotifyCollectionChanged?)GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether collection is mutating.
    /// </summary>
    private bool CollectionChangedSuspended { get; set; }

    /// <inheritdoc/>
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
    }

    /// <summary>
    /// PropertyChanged handler for DependencyProperty "SelectedItems".
    /// </summary>
    private static void OnSelectedItemsPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
    {
        if (args.NewValue is INotifyCollectionChanged collection && target is ListBoxMultiselectionBehaviour listBoxMultiselectionBehaviour)
        {
            collection.CollectionChanged += listBoxMultiselectionBehaviour.ContextSelectedItems_CollectionChanged;

            if (listBoxMultiselectionBehaviour.SelectedItems is IEnumerable enumerable)
            {
                listBoxMultiselectionBehaviour.CollectionChangedSuspended = true;
                foreach (object? item in enumerable)
                {
                    listBoxMultiselectionBehaviour.AssociatedObject.SelectedItems.Add(item);
                }

                listBoxMultiselectionBehaviour.CollectionChangedSuspended = false;
            }
        }
    }

    /// <summary>
    /// Fires when ViewModel's SelectedItems collection is changed.
    /// </summary>
    private void ContextSelectedItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (CollectionChangedSuspended)
        {
            return;
        }

        CollectionChangedSuspended = true;

        if (e.NewItems != null)
        {
            foreach (object? item in e.NewItems)
            {
                AssociatedObject.SelectedItems.Add(item);
            }
        }

        if (e.OldItems != null)
        {
            foreach (object? item in e.OldItems)
            {
                AssociatedObject.SelectedItems.Remove(item);
            }
        }

        CollectionChangedSuspended = false;
    }

    /// <summary>
    /// Fires when ListBox's SelectedItems collection is changed.
    /// </summary>
    private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CollectionChangedSuspended)
        {
            return;
        }

        CollectionChangedSuspended = true;

        if (SelectedItems is IList list)
        {
            if (e.AddedItems != null)
            {
                foreach (object? item in e.AddedItems)
                {
                    list.Add(item);
                }
            }

            if (e.RemovedItems != null)
            {
                foreach (object? item in e.RemovedItems)
                {
                    list.Remove(item);
                }
            }
        }

        CollectionChangedSuspended = false;
    }
}
