using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Fluver.UI.Controls;

[TemplatePart(Name = LayoutRootTemplateName, Type = typeof(Grid))]
public sealed partial class MeasuredComboBox : ComboBox
{
    private const string LayoutRootTemplateName = "LayoutRoot";

    private double _rightInset;

    public MeasuredComboBox()
    {
        DefaultStyleKey = typeof(MeasuredComboBox);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild(LayoutRootTemplateName) is Grid { ColumnDefinitions: { Count: >= 2 } columns })
        {
            _rightInset = columns.Skip(count: 1).Sum(c => c.Width.Value);
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var desiredSize = base.MeasureOverride(availableSize);

        if (ContainerFromItem(SelectedItem) is not ComboBoxItem { Padding: { } padding } item ||
            (item.ContentTemplateRoot ?? item.Content) is not UIElement content)
        {
            return desiredSize;
        }

        content.Measure(availableSize with { Width = Math.Max(availableSize.Width - padding.Left - padding.Right - _rightInset, 0) });
        return new(desiredSize.Width, Math.Max(content.DesiredSize.Height + padding.Top + padding.Bottom, desiredSize.Height));
    }
}
