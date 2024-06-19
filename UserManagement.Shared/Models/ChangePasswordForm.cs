namespace UserManagement.Shared.Models;

public class ChangePasswordForm
{
    public string OldPass { get; set; }
    public string NewPass { get; set; }
    public string NewPassConfirmation { get; set; }
}