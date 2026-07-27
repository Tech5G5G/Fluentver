using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace Fluver.Navigation;

public abstract class NavigationService<T> : INavigationService<T> where T : Enum
{
    public abstract T Page { get; }

    protected abstract IReadOnlyDictionary<T, Type> PageTypes { get; }

    protected Frame Frame => _frame;
    private Frame _frame;

    public void Navigate(T page, object parameter = null, Transition transition = Transition.None)
    {
        if (transition == Transition.None)
        {
            _frame.Navigate(PageTypes[page], parameter);
        }
        else
        {
            _frame.Navigate(
                PageTypes[page],
                parameter,
                transition switch
                {
                    Transition.Entrance => new EntranceNavigationTransitionInfo(),

                    Transition.SlideFromLeft or Transition.SlideFromRight or Transition.SlideFromBottom =>
                        new SlideNavigationTransitionInfo
                        {
                            Effect = (SlideNavigationTransitionEffect)transition
                        },

                    _ => new SuppressNavigationTransitionInfo()
                });
        }
    }

    // public void GoBack()
    // {
    //     if (_frame.CanGoBack)
    //     {
    //         _frame.GoBack();
    //     }
    // }

    public void SetFrame(Frame frame)
    {
        if (_frame is not null)
        {
            throw new InvalidOperationException($"Cannot set the frame of a {nameof(NavigationService<>)} more than once.");
        }

        _frame = frame;
    }
}
