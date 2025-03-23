using Windows.System;

namespace Fluentver.Helpers
{
    public static class UserHelper
    {
        public static async Task<IReadOnlyList<User>> GetUsersAsync() => await User.FindAllAsync();

        public static async Task<string> GetCurrentUserInfoAsync(string userProperty)
        {
            var user = await GetCurrentUserAsync();
            var data = await user.GetPropertyAsync(userProperty);
            return (string)data;
        }

        public static async Task<BitmapImage> GetCurrentUserPictureAsync(UserPictureSize pictureSize)
        {
            var user = await GetCurrentUserAsync();
            var picture = await user.GetPictureAsync(pictureSize);
            var picStream = await picture.OpenReadAsync();

            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(picStream);

            return bitmapImage;
        }

        public static async Task<User> GetCurrentUserAsync()
        {
            IReadOnlyList<User> users = await User.FindAllAsync();
            return users.Where(p => p.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated && p.Type == UserType.LocalUser).FirstOrDefault();
        }
    }
}
