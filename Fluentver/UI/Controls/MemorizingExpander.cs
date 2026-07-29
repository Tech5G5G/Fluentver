using Windows.Foundation.Collections;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Windows.Storage;

namespace Fluver.UI.Controls;

public partial class MemorizingExpander : Expander
{
    private static bool _triedStorage;
    private static IPropertySet Storage
    {
        get
        {
            if (field is null && !_triedStorage)
            {
                if (ApplicationData.Current is { LocalSettings: { } settings } &&
                    settings.CreateContainer("MemorizingExpander.Storage", ApplicationDataCreateDisposition.Always) is { Values: { } values })
                {
                    field = values;
                }
                _triedStorage = true;
            }

            return field;
        }
    }

    private bool _hookedEvents;

    public MemorizingExpander()
    {
        DefaultStyleKey = typeof(MemorizingExpander);
    }

    public string Id
    {
        get => (string)GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }

    public static DependencyProperty IdProperty { get; } =
        DependencyProperty.Register(nameof(Id), typeof(string), typeof(MemorizingExpander), new(defaultValue: string.Empty, OnIdChanged));

    private static void OnIdChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not MemorizingExpander expander || e.NewValue is not string id)
        {
            return;
        }

        if (Storage.TryGetValue(id, out var state))
        {
            expander.IsExpanded = (bool)state;
        }
        else
        {
            Storage[expander.Id] = expander.IsExpanded;
        }

        if (!expander._hookedEvents)
        {
            expander.Expanding += OnExpanding;
            expander.Collapsed += OnCollapsed;

            expander._hookedEvents = true;
        }
    }
    
    private static void OnExpanding(Expander sender, ExpanderExpandingEventArgs e)
    {
        Storage[(sender as MemorizingExpander).Id] = true;
    }

    private static void OnCollapsed(Expander sender, ExpanderCollapsedEventArgs e)
    {
        Storage[(sender as MemorizingExpander).Id] = false;
    }
}
