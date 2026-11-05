using Windows.ApplicationModel;

namespace Fluver.Contracts;

public interface IPackageInformation
{
    string DisplayName { get; }

    PackageVersion Version { get; }
}
