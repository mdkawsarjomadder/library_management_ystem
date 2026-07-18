using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public class ErrorViewModel
{
    [Display(Name = "Request ID")]
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
