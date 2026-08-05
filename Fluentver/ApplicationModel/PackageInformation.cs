using Windows.ApplicationModel;

namespace Fluver.ApplicationModel;

public sealed class PackageInformation : IPackageInformation
{
    public string DisplayName { get; } = Package.Current.DisplayName;

    public PackageVersion Version { get; } = Package.Current.Id.Version;
}
