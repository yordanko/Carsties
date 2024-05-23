using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services
{
    /// <summary>
    /// NOTE: This class inherits from IProfileService and overrides GetProfileDataAsync
    /// This class will add additional claims to default claims in IdentityServer
    /// </summary>
    public class CustomProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Implement this method to add additional claims to JWT tocken
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            var existingClaims = await _userManager.GetClaimsAsync(user);
            var claims = new List<Claim>
            {
                new Claim("username", user.UserName)
            };

            context.IssuedClaims.AddRange(claims);
            context.IssuedClaims.Add(existingClaims.FirstOrDefault(x=>x.Type == JwtClaimTypes.Name));
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}