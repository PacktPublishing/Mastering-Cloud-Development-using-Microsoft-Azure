using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMVeryBad
{
    class Program
    {
        static void Main(string[] args)
        {
        }



        public static AuthUser CheckIdentity(string username, string password)
        {
            using (var ctx = new UserContext())
            {
                var user = ctx.Users.Find(username);
                if (user != null && user.Password == password) return user;
                else return null;
            }
        }

    }
}
