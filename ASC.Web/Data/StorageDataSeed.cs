using ASC.Web.Configuration;
using ASC.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ASC.Web.Data
{
    public interface IIdentitySeed
	{
		Task Seed(IServiceProvider serviceProvider, IOptions<ApplicationSettings> options);
	}


	public static class IdentitySeed
	{
		public static void Seed(IServiceProvider serviceProvider, IOptions<ApplicationSettings> options)
		{
			UserManager<ApplicationUser> userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
			RoleManager<IdentityRole> roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
			// Get All comma-separated roles
			var roles = options.Value.Roles.Split(",");

			// Create roles if they are not existed
			foreach (var role in roles)
			{
				if (!roleManager.RoleExistsAsync(role).Result)
				{
					var storageRole = new IdentityRole
					{
						Name = role
					};
					IdentityResult roleResult = roleManager.CreateAsync(storageRole).Result;
				}
			}

			// Create admin if he is not existed
			var admin = userManager.FindByEmailAsync(options.Value.AdminEmail).Result;
			if (admin == null)
			{
				ApplicationUser user = new ApplicationUser
				{
					UserName = options.Value.AdminName,
					Email = options.Value.AdminEmail,
					EmailConfirmed = true,
					IsActive = "True"
				};

				IdentityResult result = userManager.CreateAsync(user, options.Value.AdminPassword).Result;
				userManager.AddClaimAsync(user, new System.Security.Claims.Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", options.Value.AdminEmail));
				userManager.AddClaimAsync(user, new System.Security.Claims.Claim("IsActive", "True"));

				// Add Admin to Admin roles
				if (result.Succeeded)
				{
					userManager.AddToRoleAsync(user, "Admin");
				}
			}

			// Create a service engineer if he is not existed
			var engineer = userManager.FindByEmailAsync(options.Value.EngineerEmail).Result;
			if (engineer == null)
			{
				ApplicationUser user = new ApplicationUser
				{
					UserName = options.Value.EngineerName,
					Email = options.Value.EngineerEmail,
					EmailConfirmed = true,
					LockoutEnabled = false,
					IsActive = "True"
				};

				IdentityResult result = userManager.CreateAsync(user, options.Value.EngineerPassword).Result;
				userManager.AddClaimAsync(user, new System.Security.Claims.Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", options.Value.EngineerEmail));
				userManager.AddClaimAsync(user, new System.Security.Claims.Claim("IsActive", "True"));

				// Add Service Engineer to Engineer role
				if (result.Succeeded)
				{
					userManager.AddToRoleAsync(user, "Engineer");
				}
			}
		}
	}
}
