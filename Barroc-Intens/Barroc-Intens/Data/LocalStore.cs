using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    internal static class LocalStore
    {
        private static User LoggedInUser { get; set; }

        internal static void SetLoggedInUser(User user)
        {
            LoggedInUser = user;
        }
        internal static User GetLoggedInUser()
        {
            return LoggedInUser;
        }
    }
}
