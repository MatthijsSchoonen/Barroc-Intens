using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    internal class LocalStore
    {
        private User LoggedInUser { get; set; }

        internal void SetLoggedInUser(User user)
        {
            this.LoggedInUser = user;
        }
        internal User GetLoggedInUser()
        {
            return this.LoggedInUser;
        }
    }
}
