using Prism.Regions;

namespace Cooking.WPF.Views;

/// <summary>
/// Logic for <see cref="TagListView"/>.
/// </summary>
public partial class TagListView : IRegionMemberLifetime
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TagListView"/> class.
    /// </summary>
    public TagListView()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    public bool KeepAlive => true;
}
