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

namespace eGym.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly IEquipmentService _equipmentService;
        private readonly IGymService _gymService;
        private UserManager<UserModel> userManager;
        public EquipmentController(IEquipmentService equipmentService, IGymService gymService, UserManager<UserModel> userManager)
        {
            _equipmentService = equipmentService;
            _gymService = gymService;
            this.userManager = userManager;
        }

        [Authorize(Roles="admin")]
        public IActionResult Add()
        {
            var data = this._gymService.List().ToList();
            ViewBag.GymList = new SelectList(data, "Id", "GymName");

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Add(EquipmentModel model)
        {
            model.LastModifiedDate = DateTime.Now;
            if (!ModelState.IsValid)
                return View(model);

            var result = _equipmentService.Add(model);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Equipment added: {model.Id}", currentUserId);
                return RedirectToAction("EquipmentList", new { id = model.GymId });
            }
            else
            {
                return View(model);
            }
        }

        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> EditAsync(string id)
        {
            var data = _equipmentService.GetById(id);
            if (data == null)
            {
                return NotFound();
            }

            var currentUserId1 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data2 = await userManager.FindByIdAsync(currentUserId1);
            if (data2.GymId != data.GymId && !User.IsInRole("admin"))
            {
                return NotFound();
            }
            var data1 = this._gymService.List().ToList();
            ViewBag.GymList = new SelectList(data1, "Id", "GymName");
            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> UpdateAsync(EquipmentModel model)
        {
            var currentUserId1 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data2 = await userManager.FindByIdAsync(currentUserId1);
            if (data2.GymId != model.GymId && !User.IsInRole("admin"))
            {
                return NotFound();
            }

            model.LastModifiedDate = DateTime.Now;
            if (!ModelState.IsValid)
                return View(model);

            var result = _equipmentService.Update(model);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Equipment updated: {model.Id}", currentUserId);
                if (User.IsInRole("admin"))
                {
                    return RedirectToAction("EquipmentList", new { id = model.GymId });
                }
                else
                {
                    return RedirectToAction("EquipmentList");
                }
            }
            else
            {
                return View(model);
            }
        }
        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> DetailsAsync(string id)
        {
            var data = _equipmentService.GetById(id);

            var currentUserId1 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data2 = await userManager.FindByIdAsync(currentUserId1);
            if (data2.GymId != data.GymId && !User.IsInRole("admin"))
            {
                return NotFound();
            }

            if (data == null)
            {
                return NotFound();
            }
            var data1 = _gymService.GetById(data.GymId);
            ViewBag.Gym = data1.GymName;

            return View(data);
        }
        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> EquipmentListAsync(int id)
        {
            var currentUserId1 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data1 = await userManager.FindByIdAsync(currentUserId1);
            var data2 = this._gymService.List().ToList();
            ViewBag.Gym = new SelectList(data2, "Id", "GymName");
            if (id != 0 && User.IsInRole("employee") && data1.GymId !=id)
            {
                return NotFound();
            }
                if (id==0 && User.IsInRole("employee"))
            {
                var equipment1 = this._equipmentService.List().Where(equipment => equipment.GymId == data1.GymId).OrderByDescending(equipment => equipment.LastModifiedDate).ToList();
                return View(equipment1);
            }
            var data = this._equipmentService.List().Where(equipment => equipment.GymId == id).OrderByDescending(equipment => equipment.LastModifiedDate).ToList();
            if(data.Count==0 && id == 0 && User.IsInRole("admin"))
            {
                data = this._equipmentService.List().OrderByDescending(equipment => equipment.LastModifiedDate).ToList();
            }
            return View(data);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Delete(string id)
        {
            var data = _equipmentService.GetById(id);
            var id1 = data.GymId;
            var result = _equipmentService.Delete(id);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Equipment deleted: {id}", currentUserId);
            }
            return RedirectToAction("EquipmentList", new {id=id1});
        }
    }
}
