using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Animation;

namespace Fluver.Navigation;

public abstract class NavigationService<T> : INavigationService<T> where T : Enum
{
    public abstract T CurrentPage { get; }

    public event NavigatedEventHandler Navigated;

    protected abstract T None { get; }

    protected abstract IReadOnlyDictionary<T, Type> PageTypes { get; }

    protected Frame Frame => _frame;
    private Frame _frame;

    protected virtual NavigationTransitionInfo DetermineTransition(T oldPage, T newPage)
    {
        return new EntranceNavigationTransitionInfo();
    }

    public void Navigate(T page)
    {
        Navigate(page, parameter: null);
    }

    public virtual void Navigate(T page, object parameter)
    {
        var currentPage = CurrentPage;

        // Avoid repeated navigation to the same page
        if (page.Equals(currentPage))
        {
            return;
        }

        if (page.Equals(None))
        {
            _frame.Content = null;
        }
        else
        {
            _frame.Navigate(
                PageTypes[page],
                parameter,
                DetermineTransition(currentPage, page));
        }
    }

    public virtual void GoBack()
    {
        if (_frame.CanGoBack)
        {
            _frame.GoBack();
        }
    }

    public virtual void GoForward()
    {
        if (_frame.CanGoForward)
        {
            _frame.GoForward();
        }
    }

    public void SetFrame(Frame frame)
    {
        if (_frame is not null)
        {
            throw new InvalidOperationException($"Cannot set the frame of a {nameof(NavigationService<>)} more than once.");
        }

        _frame = frame;
        _frame.Navigated += OnNavigated;

        void OnNavigated(object sender, NavigationEventArgs e)
        {
            Navigated?.Invoke(sender, e);
        }
    }
}
