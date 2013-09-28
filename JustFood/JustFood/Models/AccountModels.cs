using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace JustFood.Models {
    public class ChangePasswordModel {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordModel {
        [Required]
        [DataType(DataType.EmailAddress)]
        [MaxLength(200)]
        public string Email { get; set; }
    }

    public class LoginModel {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel {
        [MinLength(10)]
        [Required]
        public string Code { get; set; }

        [RegularExpression(@"[A-Za-z][A-Za-z0-9._]{3,80}", ErrorMessage = "Your given log is not valid. Make sure there is no space and it doesn't start with a number.")]
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[0-9]).{6,32}$", ErrorMessage = "Your given password is not valid and your password must be enclosed with minimum of 6 digit character including a number and any other letters.")]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        [RegularExpression(@"^(?=.*[a-z])(?=.*[0-9]).{6,32}$", ErrorMessage = "Your given password is not valid and your password must be enclosed with minimum of 6 digit character including a number and any other letters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        
        [MaxLength(30)]
        [DisplayName("Name")]
        [Required]
        public string PersonName { get; set; }
    }
}