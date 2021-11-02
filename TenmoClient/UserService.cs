using TenmoClient.Data;

namespace TenmoClient
{
    public static class UserService
    {
        private static API_User user = new API_User();

        public static void SetLogin(API_User u)
        {
            user = u;
        }

        public static void ClearLoggedInUser()
        {
            SetLogin(new API_User());
        }

        public static int UserId
        {
            get 
            {
                return user.UserId;
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
