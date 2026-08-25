using Windows.Storage;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Fluver.UI.Controls;

public sealed partial class PersistentExpander : Expander
{
    private static bool _triedStorage;
    private static IPropertySet Storage
    {
        get
        {
            if (field is null && !_triedStorage)
            {
                if (ApplicationData.Current is { LocalSettings: { } settings } &&
                    settings.CreateContainer("PersistentExpander.Storage", ApplicationDataCreateDisposition.Always) is { Values: { } values })
                {
                    field = values;
                }
                _triedStorage = true;
            }

            return field;
        }
    }

    private bool _hookedEvents;

    public PersistentExpander()
    {
        DefaultStyleKey = typeof(PersistentExpander);
    }

    public string Id
    {
        get => (string)GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }

    public static DependencyProperty IdProperty { get; } =
        DependencyProperty.Register(nameof(Id), typeof(string), typeof(PersistentExpander), new(defaultValue: string.Empty, OnIdChanged));

    private static void OnIdChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not PersistentExpander expander)
        {
            return;
        }

        if (e.NewValue is not string id || string.IsNullOrWhiteSpace(id))
        {
            expander.Expanding -= OnExpanding;
            expander.Collapsed -= OnCollapsed;

            expander._hookedEvents = false;
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
        Storage[(sender as PersistentExpander).Id] = true;
    }

    private static void OnCollapsed(Expander sender, ExpanderCollapsedEventArgs e)
    {
        Storage[(sender as PersistentExpander).Id] = false;
    }
}
