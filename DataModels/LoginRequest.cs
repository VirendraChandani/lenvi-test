using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TransactionManager.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "UserName Required")]
        [DefaultValue("lenvi")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password Required")]
        [DefaultValue("P4ssw0rd")]
        public string Password { get; set; }
    }
}
