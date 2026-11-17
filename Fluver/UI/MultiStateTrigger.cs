using Microsoft.UI.Xaml;

namespace Fluver.UI;

public sealed class MultiStateTrigger : StateTriggerBase
{
    public bool? Condition1
    {
        get => (bool?)GetValue(Condition1Property);
        set => SetValue(Condition1Property, value);
    }

    public static DependencyProperty Condition1Property { get; } =
        DependencyProperty.Register(nameof(Condition1), typeof(bool?), typeof(MultiStateTrigger), new(defaultValue: null, OnPropertyChanged));

    public bool? Condition2
    {
        get => (bool?)GetValue(Condition2Property);
        set => SetValue(Condition2Property, value);
    }

    public static DependencyProperty Condition2Property { get; } =
        DependencyProperty.Register(nameof(Condition2), typeof(bool?), typeof(MultiStateTrigger), new(defaultValue: null, OnPropertyChanged));

    public bool? Condition3
    {
        get => (bool?)GetValue(Condition3Property);
        set => SetValue(Condition3Property, value);
    }

    public static DependencyProperty Condition3Property { get; } =
        DependencyProperty.Register(nameof(Condition3), typeof(bool?), typeof(MultiStateTrigger), new(defaultValue: null, OnPropertyChanged));

    public TriggerCondition Invert
    {
        get => (TriggerCondition)GetValue(InvertProperty);
        set => SetValue(InvertProperty, value);
    }

    public static DependencyProperty InvertProperty { get; } =
        DependencyProperty.Register(nameof(Invert), typeof(TriggerCondition), typeof(MultiStateTrigger), new(defaultValue: TriggerCondition.None, OnPropertyChanged));

    private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        (sender as MultiStateTrigger)?.Evaluate();
    }

    private void Evaluate()
    {
        SetActive(
            CheckCondition(Condition1, TriggerCondition.Condition1) &&
            CheckCondition(Condition2, TriggerCondition.Condition2) &&
            CheckCondition(Condition3, TriggerCondition.Condition3));

        bool CheckCondition(bool? condition, TriggerCondition flag)
        {
            if (condition is { } boolean)
            {
                return Invert.HasFlag(flag) ? !boolean : boolean;
            }

            return true;
        }
    }
}
