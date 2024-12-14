using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    public class User
    {
        public static User LoggedInUser { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public Role Role { get; set; }
        public ICollection<Company> Companies { get; set; }
        public ICollection<Note> Notes { get; set; }
        public int RoleId { get; set; }


        public static void ClearUser()
        {
            LoggedInUser = null;
        }
    }

}
