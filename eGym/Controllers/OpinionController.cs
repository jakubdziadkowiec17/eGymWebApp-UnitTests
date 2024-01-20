using Microsoft.AspNetCore.Mvc;
using eGym.Repositories;
using eGym.Models;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace eGym.Controllers
{
    public class OpinionController : Controller
    {
        private readonly IGymService _gymService;
        private readonly IOpinionService _opinionService;
        private UserManager<UserModel> userManager;
        public OpinionController(IGymService gymService, UserManager<UserModel> userManager, IOpinionService opinionService)
        {
            _gymService = gymService;
            _opinionService = opinionService;
            this.userManager = userManager;
        }


        [Authorize(Roles = "client")]
        public async Task<IActionResult> AddAsync(int id, OpinionModel model)
        {
            var currentUser = await userManager.GetUserAsync(User);
            var data = _gymService.GetById(id);
            ViewBag.Gym = data.GymName + " " + data.Locality;
            var data1 = _opinionService.GetRecord(currentUser.Id,data.Id);
            
            if (data1==null)
            {
                model.GymId = data.Id;
                return View(model);
            }
            else
            {
                if (data1.UserId != currentUser.Id)
                {
                    return NotFound();
                }
                model = data1;
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> AddAsync(OpinionModel model)
        {
            var currentUser = await userManager.GetUserAsync(User);
            var data1 = _opinionService.GetRecord(currentUser.Id, model.GymId);
            if (data1 != null)
            {
                data1.UserId = currentUser.Id;
                data1.Name = currentUser.Name;
                data1.OpinionDate = DateTime.Now;
                data1.Rating=model.Rating;
                data1.Comment = model.Comment;
                var result = await _opinionService.Update(data1);
                if (result)
                {
                    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var logModel = new LogModel($"Opinion updated: {data1.Id}", currentUserId);
                    return RedirectToAction("GymList", "Gym");
                }
            }
            else
            { 
                model.UserId = currentUser.Id;
                model.Name = currentUser.Name;
                model.OpinionDate = DateTime.Now;

                var result = await _opinionService.Add(model);
                if (result)
                {
                    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var logModel = new LogModel($"Opinion added: {model.Id}", currentUserId);
                    return RedirectToAction("GymList", "Gym");
                }
            }
            return View(model);
        }
        public async Task<IActionResult> OpinionListAsync(int id)
        {
            ViewBag.GymId = id;
            var data1 = _gymService.GetById(id);
            ViewBag.Gym = data1.GymName + " " + data1.Locality;
            var data = this._opinionService.List().ToList();

            if (data != null)
            {
                    data = data.Where(t => t.GymId == id).ToList();
                    return View(data);
            }
            return NotFound();
        }
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var data = _opinionService.GetById(id);
            var data1 = _gymService.GetById(data.GymId);
            ViewBag.Gym = data1.GymName + " " + data1.Locality;
            if (data != null)
            {
                return View(data);
            }
            return NotFound();
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var data = _opinionService.GetById(id);
            var temp = data.GymId;
            if (data != null)
            {
                var result = _opinionService.Delete(id);
                if (result)
                {
                    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var logModel = new LogModel($"Opinion deleted: {id}", currentUserId);
                }
                return RedirectToAction("OpinionList", new { id = temp });
            }
            return NotFound();
        }
    }
}
