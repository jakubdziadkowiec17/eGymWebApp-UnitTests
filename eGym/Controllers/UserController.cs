using eGym.Models;
using eGym.Repositories;
using eGym.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;

namespace eGym.Controllers
{
    public class UserController : Controller
    {
        private IUserService authService;
        private UserManager<UserModel> userManager;
        private readonly IGymService _gymService;
        public UserController(IUserService authService, UserManager<UserModel> userManager, IGymService gymService)
        {
            this.authService = authService;
            this.userManager = userManager;
            _gymService = gymService;
        }

        [Authorize(Roles = "admin, employee, client")]
        public async Task<IActionResult> Logout()
        {
            await authService.LogoutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ResetPasswordAdminAsync(string id)
        {
            var data = await userManager.FindByIdAsync(id);
            if (data != null)
            {
                ResetPasswordAdminModel model = new ResetPasswordAdminModel();
                model.UserId = id;
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ResetPasswordAdmin(ResetPasswordAdminModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This user exists");
                    return View(model);
                }

                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var result = await userManager.ResetPasswordAsync(user, token, model.Password);
                if (result.Succeeded)
                {
                    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var logModel = new LogModel($"User password reset: {user.Id}", currentUserId);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await authService.LoginAsync(model);
            if (result.StatusCode==1)
                return RedirectToAction("Index", "Home");
            else
            {
                return RedirectToAction("Login");
            }
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var result = await authService.RegisterAsync(model);
            if(result.StatusCode==1)
            {
                var logModel = new LogModel($"User registered: {model.Username}", "Unknown");
            }
            return RedirectToAction("Index", "Home");
        }
        
        [Authorize(Roles = "admin, employee, client")]
        public async Task<IActionResult> Edit()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            if (User.IsInRole("employee"))
            {
                var data1 = _gymService.GetById(user.GymId);
                ViewBag.GymName4 = data1.GymName;
            }

            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "admin, employee, client")]
        public async Task<IActionResult> Edit(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await userManager.GetUserAsync(User);

                if (currentUser == null)
                {
                    return NotFound();
                }

                currentUser.Name = model.Name;
                currentUser.LastName = model.LastName;
                currentUser.PhoneNumber = model.PhoneNumber;
                currentUser.Address = model.Address;

                var result = authService.Update(currentUser);
                if (result)
                {
                    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var logModel = new LogModel($"User edited: {currentUser.Id}", currentUserId);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Action failed.");
                }
            }
            return View(model);
        }
        [Authorize(Roles = "admin, employee, client")]
        public async Task<IActionResult> ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "admin, employee, client")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await userManager.GetUserAsync(User);

                if (currentUser != null)
                {
                    var isOldPasswordCorrect = await userManager.CheckPasswordAsync(currentUser, model.OldPassword);

                    if (isOldPasswordCorrect)
                    {
                        var passwordHash = userManager.PasswordHasher.HashPassword(currentUser, model.Password);
                        currentUser.PasswordHash = passwordHash;

                        var result = authService.Update(currentUser);
                        if (result)
                        {
                            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            var logModel = new LogModel($"User password reset: {currentUser.Id}", currentUserId);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Błąd: Stare hasło jest niepoprawne.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Błąd: Nie można znaleźć użytkownika.");
                }
            }
            return View(model);
        }
        [Authorize(Roles = "admin, employee")]
        public async Task<ActionResult> ClientListAsync()
        {
            var users1 = await userManager.GetUsersInRoleAsync("client");
            List<UserModel> client = new List<UserModel>();

            foreach (var user in users1)
            {
                client.Add(new UserModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Name = user.Name,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address
                });
            }

            return View(client);
        }

        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> DetailsAsync(string id)
        {
            var data = await authService.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var result = await authService.Delete(id);
            if(result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"User deleted: {id}", currentUserId);
            }
            return RedirectToAction("ClientList");
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditUserAsync(string id)
        {
            var data = await authService.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditUser(UserModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var currentUser = await userManager.FindByIdAsync(model.Id);

            if (currentUser == null)
            {
                return NotFound();
            }

            currentUser.Name = model.Name;
            currentUser.LastName = model.LastName;
            currentUser.PhoneNumber = model.PhoneNumber;
            currentUser.Address = model.Address;

            var result = authService.Update(currentUser);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"User edited: {currentUser.Id}", currentUserId);
                return RedirectToAction("ClientList");
            }
            else
            {
                return View(model);
            }
        }



        [Authorize(Roles = "admin")]
        public IActionResult EmployeeAdd()
        {
            var data = this._gymService.List().ToList();

            ViewBag.GymList = new SelectList(data, "Id", "GymName");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EmployeeAddAsync(RegisterModel model)
        {
            var result = await authService.Add(model);
            if (result.StatusCode == 1)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Employee added: {model.Username}", currentUserId);
                return RedirectToAction("EmployeeList");
            }
            else
            {
                return View(model);
            }
        }

        [Authorize(Roles = "admin, employee")]
        public async Task<ActionResult> EmployeeListAsync(int id)
        {
            var currentUserId4 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data4 = await userManager.FindByIdAsync(currentUserId4);
            if(data4.GymId != id && User.IsInRole("employee") && id !=0)
            {
                return NotFound();
            }

            var users1 = await userManager.GetUsersInRoleAsync("employee");
            List<UserModel> employee = new List<UserModel>();
            var gyms = _gymService.List().ToList();
            foreach (var user in users1)
            {
                if (id==0 && User.IsInRole("admin"))
                {
                    employee.Add(new UserModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Name = user.Name,
                        LastName = user.LastName,
                        Email = user.Email,
                        GymId = user.GymId,
                        PhoneNumber = user.PhoneNumber,
                        Address = user.Address
                    });
                }
                else if (id == 0 && User.IsInRole("employee"))
                {
                    var currentUserId1 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var user4 = await userManager.FindByIdAsync(currentUserId1);
                    id =user4.GymId;
                    if (user.GymId == id)
                    {
                        employee.Add(new UserModel
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Name = user.Name,
                            LastName = user.LastName,
                            Email = user.Email,
                            GymId = user.GymId,
                            PhoneNumber = user.PhoneNumber,
                            Address = user.Address
                        });
                    }
                }
                else if (user.GymId==id)
                {
                    employee.Add(new UserModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Name = user.Name,
                        LastName = user.LastName,
                        Email = user.Email,
                        GymId = user.GymId,
                        PhoneNumber = user.PhoneNumber,
                        Address = user.Address
                    });
                }
            }
            var gymDictionary = gyms.ToDictionary(g => g.Id, g => g.GymName);
            ViewBag.GymDictionary = gymDictionary;

            return View(employee);
        }

        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> EmployeeDetailsAsync(string id)
        {
            var gyms = _gymService.List().ToList();
            var data = await authService.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            var gymDictionary = gyms.ToDictionary(g => g.Id, g => g.GymName);
            ViewBag.GymDictionary = gymDictionary;
            return View(data);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EmployeeDeleteAsync(string id)
        {
            var result = await authService.Delete(id);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Employee deleted: {id}", currentUserId);
            }
            return RedirectToAction("EmployeeList");
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EmployeeEditAsync(string id)
        {
            var data1 = this._gymService.List().ToList();
            ViewBag.GymList = new SelectList(data1, "Id", "GymName");

            var data = await authService.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EmployeeEdit(UserModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var currentUser = await userManager.FindByIdAsync(model.Id);

            if (currentUser == null)
            {
                return NotFound();
            }

            currentUser.Name = model.Name;
            currentUser.LastName = model.LastName;
            currentUser.GymId = model.GymId;
            currentUser.PhoneNumber = model.PhoneNumber;
            currentUser.Address = model.Address;

            var result = authService.Update(currentUser);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Employee edited: {model.Id}", currentUserId);
                return RedirectToAction("EmployeeList");
            }
            else
            {
                return View(model);
            }
        }
    }
}
