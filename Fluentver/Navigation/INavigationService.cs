using Microsoft.UI.Xaml.Controls;

namespace Fluver.Navigation;

public interface INavigationService<T> where T : Enum
{
    T Page { get; }

    void Navigate(T page, object parameter = null, Transition transition = Transition.None);

    // void GoBack();

    void SetFrame(Frame frame);
}

public enum Transition
{
    SlideFromBottom,
    SlideFromLeft,
    SlideFromRight,
    Entrance,
    Suppress,
    None
}
