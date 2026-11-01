using CommunityToolkit.Mvvm.ComponentModel;
using Fluver.Strings;

namespace Fluver.ViewModels;

public sealed partial class RenamerWindowViewModel(IResources resources) : ObservableObject
{
    public IResources Resources => resources;
}
