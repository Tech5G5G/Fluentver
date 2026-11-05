namespace Fluver.UI.Controls;

public abstract partial class FluverView : ViewModelPage
{
    public IList<object> ToolbarItems { get; } = [];
}
