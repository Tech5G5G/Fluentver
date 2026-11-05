using Windows.ApplicationModel;
using Fluver.Contracts;

namespace Fluver.Services;

public sealed class PackageInformation : IPackageInformation
{
    public string DisplayName { get; } = Package.Current.DisplayName;

    public PackageVersion Version { get; } = Package.Current.Id.Version;
}
