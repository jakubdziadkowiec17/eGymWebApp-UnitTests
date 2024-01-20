using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace eGym.Repositories
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserModel> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<UserModel> signInManager;
        public UserService(UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.roleManager=roleManager;
            this.signInManager=signInManager;
            this.userManager=userManager;
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();

        }

        public async Task<StatusModel> LoginAsync(LoginModel model)
        {
            var status = new StatusModel();
            var user = await userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                status.StatusCode = 0;
                return status;
            }

            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                status.StatusCode = 0;
                return status;
            }

            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, true, true);
            if (signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                status.StatusCode = 1;
            }
            else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
            }
            else
            {
                status.StatusCode = 0;
            }

            return status;
        }

        public async Task<StatusModel> RegisterAsync(RegisterModel model)
        {
            model.Role = "client";
            var status = new StatusModel();
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                status.StatusCode = 0;
                return status;
            }

            UserModel user = new UserModel()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                LastName = model.LastName,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                return status;
            }

            if (!await roleManager.RoleExistsAsync(model.Role))
                await roleManager.CreateAsync(new IdentityRole(model.Role));


            if (await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user, model.Role);
            }

            status.StatusCode = 1;
            return status;
        }

        public bool Update(UserModel model)
        {
            try
            {
                var result = userManager.UpdateAsync(model).Result;
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<StatusModel> Add(RegisterModel model)
        {
            model.Role = "employee";
            var status = new StatusModel();
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                status.StatusCode = 0;
                return status;
            }
            UserModel user = new UserModel()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                LastName = model.LastName,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                GymId=model.GymId,
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                return status;
            }

            if (!await roleManager.RoleExistsAsync(model.Role))
                await roleManager.CreateAsync(new IdentityRole(model.Role));


            if (await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user, model.Role);
            }

            status.StatusCode = 1;
            return status;
        }

        public async Task<UserModel> GetById(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            return user;
        }

        public async Task<bool> Delete(string id)
        {
            try
            {
                var user =  await userManager.FindByIdAsync(id);
                if (user == null)
                    return false;

                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

}
