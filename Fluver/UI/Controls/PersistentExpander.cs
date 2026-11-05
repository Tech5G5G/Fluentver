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

    private bool _validId;

    public PersistentExpander()
    {
        DefaultStyleKey = typeof(Expander);

        Expanding += OnExpanding;
        Collapsed += OnCollapsed;
    }

    public string PersistenceId
    {
        get => (string)GetValue(PersistenceIdProperty);
        set => SetValue(PersistenceIdProperty, value);
    }

    public static DependencyProperty PersistenceIdProperty { get; } =
        DependencyProperty.Register(nameof(PersistenceId), typeof(string), typeof(PersistentExpander), new(defaultValue: null, OnPersistenceIdChanged));

    private static void OnPersistenceIdChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not PersistentExpander expander)
        {
            return;
        }

        if (e.NewValue is not string id || string.IsNullOrWhiteSpace(id))
        {
            expander._validId = false;
            return;
        }

        expander._validId = true;

        if (Storage is { } storage)
        {
            if (storage.TryGetValue(id, out var state))
            {
                expander.IsExpanded = (bool)state;
            }
            else
            {
                storage[id] = expander.IsExpanded;
            }
        }
    }

    private static void OnExpanding(Expander sender, ExpanderExpandingEventArgs e)
    {
        if (Storage is { } storage &&
            sender is PersistentExpander { _validId: true, PersistenceId: { } id })
        {
            storage[id] = true;
        }
    }

    private static void OnCollapsed(Expander sender, ExpanderCollapsedEventArgs e)
    {
        if (Storage is { } storage &&
            sender is PersistentExpander { _validId: true, PersistenceId: { } id })
        {
            storage[id] = false;
        }
    }
}
