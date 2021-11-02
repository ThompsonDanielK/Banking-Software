using TenmoClient.Data;

namespace TenmoClient
{
    /// <summary>
    /// This is a static class and cannot be instantiated. Its job is to track information about the logged-in user.
    /// It's also not the best class in general, but you can use the UserId, IsLoggedIn, and Token methods
    /// to get information on the currently logged-in user.
    /// </summary>
    public static class UserService
    {
        private static API_User user = new API_User();

        public static void SetLogin(API_User u)
        {
            user = u ?? throw new System.ArgumentNullException(nameof(u));
        }

        public static void ClearLoggedInUser()
        {
            SetLogin(new API_User());
        }

        public static int UserId
        {
            get 
            {
                return user?.UserId ?? 0;
            }
        }

        public static bool IsLoggedIn
        {
            get
            {
                return !string.IsNullOrWhiteSpace(user.Token);
            }
        }

        public static string Token
        {
            get
            {
                return user?.Token ?? string.Empty;
            }
        }
    }
}
