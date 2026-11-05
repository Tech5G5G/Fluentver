using Fluver.Services.Navigation;

namespace Fluver.Contracts;

public interface IMainWindowNavigationService : INavigationService<MainWindowPage>;

public interface IMainPageNavigationService : INavigationService<FluverPage>;
