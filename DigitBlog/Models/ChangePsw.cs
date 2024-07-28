using System.ComponentModel.DataAnnotations;

namespace DigitBlog.Models
{
    public class ChangePsw
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Please, Enter your Current Password")]
        [Display(Name="Current Password")]
        public string CurrentPassword { get; set; } = null!;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please, Enter your New Password")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }= null!;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please, Enter your Confirm Password")]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword",ErrorMessage ="Confrim Password Does not matched.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
