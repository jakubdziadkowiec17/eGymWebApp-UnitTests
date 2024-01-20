using Microsoft.AspNetCore.Mvc;
using eGym.Repositories;
using eGym.Models;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace eGym.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private UserManager<UserModel> userManager;
        public TaskController(ITaskService taskService, UserManager<UserModel> userManager)
        {
            _taskService = taskService;
            this.userManager = userManager;
        }

        [Authorize(Roles="admin")]
        public async Task<IActionResult> AddAsync(string id, TaskModel model)
        {
            var data = await userManager.FindByIdAsync(id);
            ViewBag.User=data.Name + " " +data.LastName;
            ViewBag.UserId1 = data.Id;

            if (data != null)
            {
                model.UserId = data.Id;
                return View(model);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Add(TaskModel model)
        {
            model.CreatedDate = DateTime.Now;
            model.TaskStatus = Models.TaskStatus.Created;
            if (!ModelState.IsValid)
                return View(model);

            var result = _taskService.Add(model);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Task added: {model.Id}", currentUserId);
                return RedirectToAction("TaskList", new { id = model.UserId });
            }
            else
            {
                return View(model);
            }
        }

        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> EditAsync(int id)
        {
            var data = _taskService.GetById(id);
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId != data.UserId && !User.IsInRole("admin"))
            {
                return NotFound();
            }
            var data1 = await userManager.FindByIdAsync(data.UserId);
            ViewBag.User = data1.Name + " " + data1.LastName;

            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "admin, employee")]
        public IActionResult Update(TaskModel model)
        {
            var currentUserId1 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId1 != model.UserId && !User.IsInRole("admin"))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return View(model);

            if (model.TaskStatus==Models.TaskStatus.Completed)
            {
                model.EndDate = DateTime.Now;
            }
            if (model.TaskStatus == Models.TaskStatus.InProgress || model.TaskStatus == Models.TaskStatus.Created)
            {
                model.EndDate = null;
            }
            var result = _taskService.Update(model);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Task updated: {model.Id}", currentUserId);
                return RedirectToAction("TaskList", new { id = model.UserId });
            }
            else
            {
                return View(model);
            }
        }
        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var data = _taskService.GetById(id);

            var currentUserId1 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId1 != data.UserId && !User.IsInRole("admin"))
            {
                return NotFound();
            }

            if (data == null)
            {
                return NotFound();
            }
            var data1 = await userManager.FindByIdAsync(data.UserId);
            ViewBag.User = data1.Name + " " + data1.LastName;

            return View(data);
        }
        [Authorize(Roles = "admin, employee")]
        public async Task<IActionResult> TaskListAsync(string id)
        {
            var currentUserId1 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId1 != id && !User.IsInRole("admin"))
            {
                return NotFound();
            }

            var data1 = await userManager.FindByIdAsync(id);
            ViewBag.User = data1.Name + " " + data1.LastName;

            var data = this._taskService.List().Where(task => task.UserId == id).OrderByDescending(task => task.Id).ToList();

            return View(data);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            var data = _taskService.GetById(id);
            var id1 = data.UserId;
            var result = _taskService.Delete(id);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Task deleted: {id}", currentUserId);
            }
            return RedirectToAction("TaskList", new { id = id1 });
        }
    }
}
