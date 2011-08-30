namespace UCTemplate.Web.Mvc.ViewModels
{
    #region

    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    #endregion

    public class LoginInput
    {
        [Required]
        [Display(Name = "Username", Description = "Domain user name.")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [HiddenInput]
        public string ReturnUrl { get; set; }
    }
}
