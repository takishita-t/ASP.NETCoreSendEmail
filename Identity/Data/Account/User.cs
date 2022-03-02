using Microsoft.AspNetCore.Identity;

namespace Identity.Data.Account
{
    public class User : IdentityUser
    {
        public string Department { get; set; }
        public string Position { get; set; }
    }
}
