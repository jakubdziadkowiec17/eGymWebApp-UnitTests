using Microsoft.AspNetCore.Mvc;
using eGym.Repositories;
using eGym.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace eGym.Controllers
{
    public class GymController : Controller
    {
        private readonly IGymService _gymService;
        private readonly IOpinionService _opinionService;
        private UserManager<UserModel> userManager;
        public GymController(IGymService gymService, IOpinionService opinionService, UserManager<UserModel> userManager)
        {
            _gymService = gymService;
            _opinionService = opinionService;
            this.userManager = userManager;
        }

        [Authorize(Roles = "admin")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Add(GymModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = _gymService.Add(model);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Gym added: {model.Id}", currentUserId);
                return RedirectToAction("GymList");
            }
            else
            {
                return View(model);
            }
        }
        [Authorize(Roles = "admin")]
        public IActionResult Edit(int id)
        {
            var data = _gymService.GetById(id);
            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Update(GymModel model)
        {
            if (!ModelState.IsValid)
                return View(model);


            var result = _gymService.Update(model);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Gym updated: {model.Id}", currentUserId);
                return RedirectToAction("GymList");
            }
            else
            {
                return View(model);
            }
        }
        public IActionResult Details(int id)
        {
            var data = _gymService.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }
        public async Task<IActionResult> GymListAsync()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data4 = await userManager.FindByIdAsync(currentUserId);
            if(data4 != null)
            {
                ViewBag.UserGymId = data4.GymId;
            }
            else
            {
                ViewBag.UserGymId = "";
            }


            var data1 = this._opinionService.List().ToList();
            var data = this._gymService.List().ToList();

            var gymRating = data1.GroupBy(opinion => opinion.GymId)
        .ToDictionary(group => group.Key, group => group.Average(opinion => opinion.Rating));
            ViewBag.GymRating = gymRating;

            var gymSum = data1.GroupBy(opinion => opinion.GymId)
        .ToDictionary(group => group.Key, group => group.Count());
            ViewBag.GymSum = gymSum;

            return View(data);
        }
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            var result = _gymService.Delete(id);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Gym deleted: {id}", currentUserId);
            }
            return RedirectToAction("GymList");
        }
    }
}
