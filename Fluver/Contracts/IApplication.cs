using Windows.ApplicationModel;

namespace Fluver.Contracts;

public interface IApplication
{
    string DisplayName { get; }
    PackageVersion Version { get; }

    void Restart();
    void Exit();
}
