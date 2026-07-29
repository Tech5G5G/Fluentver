using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Fluver.Navigation;

public interface INavigationService<T> where T : Enum
{
    T CurrentPage { get; }

    event NavigatedEventHandler Navigated;

    void Navigate(T page);
    void Navigate(T page, object parameter);

    void GoBack();
    void GoForward();

    void SetFrame(Frame frame);
}
