using Microsoft.AspNetCore.Mvc;
using eGym.Repositories;
using eGym.Models;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eGym.Controllers
{
    public class ClassesController : Controller
    {
        private readonly IClassesService _classesService;
        private readonly IClassesUserService _classesUserService;
        private readonly IGymService _gymService;
        private UserManager<UserModel> userManager;
        private readonly IPaymentService _paymentService;
        public ClassesController(IClassesService classesService, IGymService gymService, UserManager<UserModel> userManager, IClassesUserService classesUserService, IPaymentService paymentService)
        {
            _classesService = classesService;
            _gymService = gymService;
            this.userManager = userManager;
            _classesUserService = classesUserService;
            _paymentService = paymentService;
        }

        [Authorize(Roles = "employee")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<IActionResult> AddAsync(ClassesModel model)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data2 = await userManager.FindByIdAsync(currentUserId);
            model.EmployeeId = currentUserId;
            model.GymId = data2.GymId;

            var result = _classesService.Add(model);
            if (result && model.StartDate > DateTime.Now && model.EndDate > DateTime.Now)
            {
                var logModel = new LogModel($"Classes added: {model.Id}", currentUserId);
                return RedirectToAction("MyClassesList");
            }
            else
            {
                return View(model);
            }
        }

        [Authorize(Roles = "employee")]
        public async Task<IActionResult> EditAsync(int id)
        {
            var data = _classesService.GetById(id);
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userManager.FindByIdAsync(data.EmployeeId);
            if (currentUserId != data.EmployeeId || data.StartDate <= DateTime.Now)
            {
                return NotFound();
            }
            var data1 = _gymService.GetById(data.GymId);
            ViewBag.Gym = data1.GymName;
            ViewBag.Trainer = user.Name + " " + user.LastName;
            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult Update(ClassesModel model)
        {
            var currentUserId1 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId1 != model.EmployeeId || model.StartDate <= DateTime.Now || model.EndDate <= DateTime.Now)
            {
                return RedirectToAction("Edit", new { id = model.Id });
            }

            if (!ModelState.IsValid)
                return View(model);

            var result = _classesService.Update(model);
            if (result)
            {
                var logModel = new LogModel($"Classes updated: {model.Id}", currentUserId1);
                return RedirectToAction("MyClassesList");
            }
            else
            {
                return View(model);
            }
        }
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var data = _classesService.GetById(id);
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userManager.FindByIdAsync(currentUserId);

            if (data == null)
            {
                return NotFound();
            }
            var data1 = _gymService.GetById(data.GymId);
            ViewBag.Gym = data1.GymName;
            if(user != null)
            {
                ViewBag.IdGym = user.GymId;
            }

            var data2 = this._classesUserService.List().Where(classes => classes.ClassesId == id).OrderByDescending(classes => classes.DateOfReservation).ToList();

            var model = new DetailsClassesModel
            {
                Classes = data,
                ClassesUser = data2
            };

            var data4 = this._gymService.List().ToList();
            ViewBag.GymList = new SelectList(data4, "Id", "GymName");

            var userList = await userManager.Users
            .Select(u => new { Id = u.Id, FullName = u.Name + " " + u.LastName })
            .ToListAsync();
            ViewBag.Users = new SelectList(userList, "Id", "FullName");

            var classesUser1 = _classesUserService.List().ToList();
            var peopleSum = classesUser1.GroupBy(people => people.ClassesId).ToDictionary(group => group.Key, group => group.Count());
            ViewBag.PeopleCount = peopleSum;

            return View(model);
        }
        public async Task<IActionResult> GymsClassesListAsync(int id)
        {
            var data = this._classesService.List().Where(classes => classes.GymId == id && classes.StartDate >= DateTime.Now).OrderBy(classes => classes.StartDate).ToList();
            
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userManager.FindByIdAsync(currentUserId);
            if (user != null)
            {
                ViewBag.IdGym = user.GymId;
            }

            var data4 = this._gymService.List().ToList();
            ViewBag.GymList = new SelectList(data4, "Id", "GymName");

            var userList = await userManager.Users
            .Select(u => new { Id = u.Id, FullName = u.Name + " " + u.LastName })
            .ToListAsync();
            ViewBag.Users = new SelectList(userList, "Id", "FullName");

            var classesUser1 = _classesUserService.List().ToList();
            var peopleSum = classesUser1.GroupBy(people => people.ClassesId).ToDictionary(group => group.Key, group => group.Count());
            ViewBag.PeopleCount = peopleSum;

            return View(data);
        }

        [Authorize(Roles = "admin, employee, client")]
        public async Task<IActionResult> MyClassesListAsync(string id)
        {
            List<ClassesModel> data = new List<ClassesModel>();
            List<ClassesUserModel>? data2 = new List<ClassesUserModel>();
            if (id == null)
            {
                if (!User.IsInRole("employee") && !User.IsInRole("client"))
                {
                    return NotFound();
                }
                id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                ViewBag.Title1 = "My classes";
            }
            else
            {
                if (id == User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                {
                    ViewBag.Title1 = "My classes";
                }
                else
                {
                    var currentUserId4 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var user4 = await userManager.FindByIdAsync(currentUserId4);
                    var user8 = await userManager.FindByIdAsync(id);
                    if (user8 == null) 
                    {
                        return NotFound();
                    }
                    if (User.IsInRole("client") || (User.IsInRole("employee") && user4.GymId != user8.GymId))
                    {
                        return NotFound();
                    }
                    ViewBag.Title1 = "Trainer's classes";
                }
            }
            if (User.IsInRole("client") && id == User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            {
                var data1 = this._classesUserService.List().Where(classes => classes.UserId == id).ToList();
                data2 = data1.Select(c => new ClassesUserModel { ClassesId = c.ClassesId, DateOfReservation = c.DateOfReservation, Id = c.Id, UserId = c.UserId, PaymentDate = c.PaymentDate, }).OrderByDescending(classes => classes.DateOfReservation).ToList();
                data = this._classesService.List().Where(classes => data2.Select(d => d.ClassesId).Contains(classes.Id)).OrderByDescending(classes => classes.StartDate).ToList();
            }
            else
            {
                data = this._classesService.List().Where(classes => classes.EmployeeId == id).OrderByDescending(classes => classes.StartDate).ToList();
                data2 = null;
            }
            ViewBag.id1 = id;

            var model = new MyClassesClientModel
            {
                Classes = data,
                ClassesUser = data2
            };

            var data4 = this._gymService.List().ToList();
            ViewBag.GymList = new SelectList(data4, "Id", "GymName");

            var userList = await userManager.Users
            .Select(u => new { Id = u.Id, FullName = u.Name + " " + u.LastName })
            .ToListAsync();
            ViewBag.Users = new SelectList(userList, "Id", "FullName");

            var classesUser1 = _classesUserService.List().ToList();
            var peopleSum = classesUser1.GroupBy(people => people.ClassesId).ToDictionary(group => group.Key, group => group.Count());
            ViewBag.PeopleCount = peopleSum;

            return View(model);
        }

        [Authorize(Roles = "admin, employee")]
        public IActionResult Delete(int id)
        {
            var data = _classesService.GetById(id);
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if((currentUserId != data.EmployeeId && User.IsInRole("employee")) || data.StartDate <= DateTime.Now)
            {
                return NotFound();
            }

            var id1 = data.EmployeeId;

            var classesUser1 = _classesUserService.List().Where(i => i.ClassesId == id).ToList();
            if (classesUser1.Count > 0)
            {
                return NotFound();
            }
            else
            {
                var result = _classesService.Delete(id);
                if (result)
                {
                    var logModel = new LogModel($"Classes deleted: {id}", currentUserId);
                }
            }
                if (User.IsInRole("employee"))
                {
                    return RedirectToAction("MyClassesList");
                }
                else
                {
                    return RedirectToAction("MyClassesList", new { id = id1 });
                }
            
        }

        [Authorize(Roles = "admin, employee, client")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var data = _classesUserService.GetById(id);
            var data1 = _classesService.GetById(data.ClassesId);
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user4 = await userManager.FindByIdAsync(currentUserId);
            if (User.IsInRole("client"))
            {
                if (data.PaymentDate != null || data1.StartDate <= DateTime.Now || currentUserId != data.UserId)
                {
                    return NotFound();
                }
            }
            if (User.IsInRole("employee"))
            {
                if (data.PaymentDate != null || user4.GymId != data1.GymId)
                {
                    return NotFound();
                }
            }
            if (User.IsInRole("admin"))
            {
                if (data.PaymentDate != null)
                {
                    return NotFound();
                }
            }

            var id1 = data.ClassesId;
            var result = _classesUserService.Delete(id);
            if (result)
            {
                var logModel = new LogModel($"MyClasses deleted: {id}", currentUserId);
            }
            if (User.IsInRole("client"))
            {
                return RedirectToAction("MyClassesList");
            }
            else
            {
                return RedirectToAction("Details", new { id = id1 });
            }
        }

        [Authorize(Roles = "client")]
        public async Task<IActionResult> ReserveAsync(int id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data = _classesService.GetById(id);

            if (data != null && data.StartDate > DateTime.Now)
            {
                var classesUser1 = _classesUserService.List().Where(i => i.ClassesId == id).ToList();
                if (classesUser1.Count >= data.NumberOfPeople)
                {
                    return RedirectToAction("MyClassesList");
                }

                var model2 = new ClassesUserModel();
                model2.UserId = currentUserId;
                model2.ClassesId = data.Id;
                model2.DateOfReservation = DateTime.Now;
                var result2 = _classesUserService.Add(model2);
                if (result2)
                {
                    var logModel2 = new LogModel($"MyClasses reserved: {model2.Id}", model2.UserId);
                }
                else
                {
                    return NotFound();
                }
            }
            return RedirectToAction("MyClassesList");
        }

        [Authorize(Roles = "employee")]
        public async Task<IActionResult> ReserveAdminAsync(int id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user2 = await userManager.FindByIdAsync(currentUserId);
            var data = _classesService.GetById(id);
            if (data != null)
            {
                var classesUser1 = _classesUserService.List().Where(i => i.ClassesId == id).ToList();
                if (classesUser1.Count >= data.NumberOfPeople || user2.GymId !=data.GymId || data.StartDate <= DateTime.Now)
                {
                    return NotFound();
                }
                var model2 = new ClassesUserModel();
                model2.ClassesId = data.Id;
                model2.DateOfReservation = DateTime.Now;
                var result2 = _classesUserService.Add(model2);
                if (result2)
                {
                    var logModel2 = new LogModel($"MyClasses reserved: {model2.Id}", currentUserId);
                }
                else
                {
                    return NotFound();
                }
            }
            return RedirectToAction("Details", new { id = id });
        }

        [Authorize(Roles = "employee")]
        public async Task<IActionResult> PayAsync(int id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data = _classesUserService.GetById(id);
            if (data != null && data.PaymentDate == null)
            {
                var data1 = _classesService.GetById(data.ClassesId);
                var user = await userManager.FindByIdAsync(data1.EmployeeId);
                if (user.GymId == data1.GymId)
                {
                    data.PaymentDate = DateTime.Now;
                    var result = _classesUserService.Update(data);
                    if (result)
                    {
                        var logModel = new LogModel($"MyClasses marked as paid: {data.Id}", currentUserId);

                        var model2 = new PaymentModel();
                        model2.GymId = data1.GymId;
                        model2.Deposit = true;
                        model2.Sum = data1.Price;
                        model2.Source = "MyClasses";
                        model2.SourceId = data.Id.ToString();
                        model2.PaymentDate = (DateTime)data.PaymentDate;
                        model2.UserEnteringThePayment = currentUserId;
                        var result2 = _paymentService.Add(model2);

                        if (result2)
                        {
                            var logModel2 = new LogModel($"Payment added: {model2.Id}", model2.UserEnteringThePayment);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
            return RedirectToAction("Details", new { id = data.ClassesId });
        }
    }
}