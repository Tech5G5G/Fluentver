﻿namespace Fluentver.Controls;

[Microsoft.UI.Xaml.Markup.ContentProperty(Name = nameof(Children))]
public partial class InfoPage : Page
{
    public IList<GlyphButton> ToolbarButtons { get; } = [];

    public Setting<ApplicationDataCompositeValue> ExpanderStates { get; set; } = SettingValues.ExpanderStates;

    public ObservableCollection<Expander> Children { get; } = [];
    readonly StackPanel content;

    public InfoPage()
    {
        Content = content = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Top,
            Spacing = 8,
            Margin = new Thickness(8, 0, 8, 0),
            ChildrenTransitions = [new RepositionThemeTransition { IsStaggeringEnabled = false }]
        };

        Children.CollectionChanged += (s, e) =>
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var expander = e.NewItems[0] as Expander;
                string key = (string)expander.Header;

                expander.IsExpanded = TryGetExpanderExpanded(key, ExpanderStates, () => UpdateExpanderExpanded(key, true, ExpanderStates));

                expander.Expanding += (s, e) => UpdateExpanderExpanded(key, true, ExpanderStates);
                expander.Collapsed += (s, e) => UpdateExpanderExpanded(key, false, ExpanderStates);

                content.Children.Add(expander);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var expander = e.NewItems[0] as Expander;
                content.Children.Remove(expander);
            }
        };
    }

    private static void UpdateExpanderExpanded(string key, bool value, Setting<ApplicationDataCompositeValue> setting)
    {
        var composite = setting.Value;
        composite[key] = value;
        setting.Value = composite;
    }

    private static bool TryGetExpanderExpanded(string key, Setting<ApplicationDataCompositeValue> setting, Action defaultUsed)
    {
        if (setting.Value.TryGetValue(key, out object value) && value is bool t)
            return t;
        else
        {
            defaultUsed();
            return true;
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is MainWindow mw)
        {
            mw.ToolbarButtons.Clear();
            foreach (var button in ToolbarButtons)
                mw.ToolbarButtons.Add(button);
        }
    }
}
