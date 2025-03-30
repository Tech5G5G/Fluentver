using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using Microsoft.Win32;

namespace Fluentver.Helpers
{
    public static class UserHelper
    {
        const string AccountPictures = SystemHelper.HKLM + @"SOFTWARE\Microsoft\Windows\CurrentVersion\AccountPicture\Users\";
        const string StoredIdentities = @".DEFAULT\Software\Microsoft\IdentityCRL\StoredIdentities";

        const string SIDStart = "S-1-5-21";
        const char BuiltInEnd = '5';

        static readonly SecurityIdentifier currentSID = WindowsIdentity.GetCurrent().Owner;

        static readonly PrincipalSearcher searcher = new(new UserPrincipal(new(ContextType.Machine)));

        static readonly RegistryKey hUsers = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Default);

        static readonly Dictionary<string, string> personalEmails = hUsers.OpenSubKey(StoredIdentities).GetSubKeyNames()
            .ToDictionary(email => hUsers.OpenSubKey($"{StoredIdentities}\\{email}").GetSubKeyNames().FirstOrDefault(string.Empty));

        /// <summary>Gets the current <see cref="UserPrincipal"/>.</summary>
        /// <returns>The <see cref="UserPrincipal"/> of the current user, asynchronously.</returns>
        public static async Task<UserPrincipal> GetCurrentUserAsync()
        {
            var users = await FindAllUsersAsync();
            return users.First(i => i.Sid == currentSID);
        }

        /// <summary>Gets an <see cref="Array"/> of <see cref="UserPrincipal"/> of all the users on the system, except for the current user.</summary>
        /// <returns>An <see cref="Array"/> of <see cref="UserPrincipal"/>, asynchronously.</returns>
        public static async Task<UserPrincipal[]> GetAllUsersAsync()
        {
            var users = await FindAllUsersAsync();
            return [.. users.Where(i => !i.Sid.Value.StartsWith(SIDStart) || i.Sid.Value[^3] != BuiltInEnd && i.Sid != currentSID).Cast<UserPrincipal>()];
        }

        private static async Task<UserPrincipal[]> FindAllUsersAsync() => await Task.Run(searcher.FindAll().Cast<UserPrincipal>().ToArray);

        /// <summary>Gets a <see cref="BitmapImage"/> representation of the <paramref name="user"/>s account picture.</summary>
        /// <param name="user">The <see cref="UserPrincipal"/> to get the account picture of.</param>
        /// <returns>The <paramref name="user"/>s account image as a <see cref="BitmapImage"/>.</returns>
        public static BitmapImage GetPicture(this UserPrincipal user)
        {
            string path = (string)Registry.GetValue(AccountPictures + user.Sid, "Image1080", string.Empty);
            return string.IsNullOrWhiteSpace(path) ? null : new() { UriSource = new(path) };
        }

        /// <summary>Looks up the email address of <paramref name="user"/> using its <see cref="Principal.Sid"/>.</summary>
        /// <param name="user">The <see cref="UserPrincipal"/> to look the email address up of.</param>
        /// <returns>The email address of <paramref name="user"/>.</returns>
        public static string GetEmailAddress(this UserPrincipal user) => personalEmails.TryGetValue(user.Sid.Value, out string email) ? email : user.EmailAddress;

        /// <summary>Finds the best name of <paramref name="user"/> to display.</summary>
        /// <param name="user">The <see cref="UserPrincipal"/> to find the best name of.</param>
        /// <returns>If it passes <see cref="string.IsNullOrWhiteSpace(string?)"/>, <see cref="Principal.DisplayName"/>. Otherwise, <see cref="Principal.Name"/></returns>
        public static string GetBestDisplayName(this UserPrincipal user) => string.IsNullOrWhiteSpace(user.DisplayName) ? user.Name : user.DisplayName;
    }
}
