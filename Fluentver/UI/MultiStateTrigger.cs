using Microsoft.UI.Xaml;

namespace Fluver.UI;

public sealed class MultiStateTrigger : StateTriggerBase
{
    public bool? ConditionOne
    {
        get => (bool?)GetValue(ConditionOneProperty);
        set => SetValue(ConditionOneProperty, value);
    }

    public static DependencyProperty ConditionOneProperty { get; } =
        DependencyProperty.Register(nameof(ConditionOne), typeof(bool?), typeof(MultiStateTrigger), new(defaultValue: null, OnPropertyChanged));

    public bool? ConditionTwo
    {
        get => (bool?)GetValue(ConditionTwoProperty);
        set => SetValue(ConditionTwoProperty, value);
    }

    public static DependencyProperty ConditionTwoProperty { get; } =
        DependencyProperty.Register(nameof(ConditionTwo), typeof(bool?), typeof(MultiStateTrigger), new(defaultValue: null, OnPropertyChanged));

    public bool? ConditionThree
    {
        get => (bool?)GetValue(ConditionThreeProperty);
        set => SetValue(ConditionThreeProperty, value);
    }

    public static DependencyProperty ConditionThreeProperty { get; } =
        DependencyProperty.Register(nameof(ConditionThree), typeof(bool?), typeof(MultiStateTrigger), new(defaultValue: null, OnPropertyChanged));

    public Condition InvertedConditions
    {
        get => (Condition)GetValue(InvertedConditionsProperty);
        set => SetValue(InvertedConditionsProperty, value);
    }

    public static DependencyProperty InvertedConditionsProperty { get; } =
        DependencyProperty.Register(nameof(InvertedConditions), typeof(Condition), typeof(MultiStateTrigger), new(defaultValue: Condition.None, OnPropertyChanged));

    private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        (sender as MultiStateTrigger)?.Evaluate();
    }

    private void Evaluate()
    {
        SetActive(
            CheckCondition(ConditionOne, Condition.One) &&
            CheckCondition(ConditionTwo, Condition.Two) &&
            CheckCondition(ConditionThree, Condition.Three));

        bool CheckCondition(bool? condition, Condition flag)
        {
            if (condition is { } boolean)
            {
                return InvertedConditions.HasFlag(flag) ? !boolean : boolean;
            }

            return true;
        }
    }
}

[Flags]
public enum Condition
{
    None = 0,
    One = 1,
    Two = 2,
    Three = 4
}
