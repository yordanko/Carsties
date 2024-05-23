// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace IdentityService.Pages.Diagnostics;

[SecurityHeaders]
[Authorize]
public class Index : PageModel
{
    public ViewModel View { get; set; } = default!;

    public async Task<IActionResult> OnGet()
    {
        //NOTE: this is list of container ip addresses.
        var localAddresses = new List<string> { "::ffff:172.26.0.5", "127.0.0.1", "::1" };
        if(HttpContext.Connection.LocalIpAddress != null)
        {
            localAddresses.Add(HttpContext.Connection.LocalIpAddress.ToString());
        }
        //if run in Docker add it to local addresses
        if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Docker"
        && !localAddresses.Contains(HttpContext.Connection.RemoteIpAddress?.ToString()))
        {
            localAddresses.Add(HttpContext.Connection.RemoteIpAddress.ToString());
        }

        View = new ViewModel(await HttpContext.AuthenticateAsync());
        View.IpAddress = HttpContext.Connection.LocalIpAddress?.ToString();


        return Page();
    }
}
