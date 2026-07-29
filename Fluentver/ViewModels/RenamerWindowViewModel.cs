using CommunityToolkit.Mvvm.ComponentModel;
using Fluver.Windows;

namespace Fluver.ViewModels;

public sealed partial class RenamerWindowViewModel(IWindowManager manager) : ObservableObject
{
    public void AddToWindowManager(RenamerWindow renamerWindow)
    {
        manager.AddWindow(renamerWindow);
    }
}
