using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barroc_Intens.Data
{
    class PasswordReset
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }

        public string Code { get; set; }
    }
}
