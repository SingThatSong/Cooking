using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cooking.Web.Pages;

/// <summary>
/// TODO: Refactor.
/// </summary>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorModel : PageModel
{
    /// <summary>
    /// Gets or sets id of request in error.
    /// </summary>
    public string? RequestID { get; set; }

    /// <summary>
    /// Gets a value indicating whether to show request ID.
    /// </summary>
    public bool ShowRequestID => !string.IsNullOrEmpty(RequestID);

    /// <summary>
    /// What to do on get.
    /// </summary>
    public void OnGet() => RequestID = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
}
