namespace webappproject.MVVM
{
    public class ChangePasswordVM
    {
        public string oldPassword { get; set; } = string.Empty;
        public string newPassword { get; set; } = string.Empty;
        public string newPasswordConfirmation { get; set; } = string.Empty;
    }
}
