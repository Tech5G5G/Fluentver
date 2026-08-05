using Windows.ApplicationModel;

namespace Fluver.ApplicationModel;

public interface IPackageInformation
{
    string DisplayName { get; }

    PackageVersion Version { get; }
}
