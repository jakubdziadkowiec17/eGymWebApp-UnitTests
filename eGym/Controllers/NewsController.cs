using Microsoft.AspNetCore.Mvc;
using eGym.Repositories;
using eGym.Models;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;

namespace eGym.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [Authorize(Roles = "admin")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Add(NewsModel model)
        {
            model.CreatedDate = DateTime.Now;
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            model.UserId = currentUserId;
            var result = _newsService.Add(model);
            if (result)
            {
                var logModel = new LogModel($"News added: {model.Id}", currentUserId);
                return RedirectToAction(nameof(NewsList));
            }
            else
            {
                return View(model);
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult Edit(int id)
        {
            var data = _newsService.GetById(id);
            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Update(NewsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.CreatedDate = DateTime.Now;
            var result = _newsService.Update(model);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"News updated: {model.Id}", currentUserId);
                return RedirectToAction(nameof(NewsList));
            }
            else
            {
                return View(model);
            }
        }
        public IActionResult Details(int id)
        {
            var data = _newsService.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }
        public IActionResult NewsList()
        {
            var data = this._newsService.List().OrderByDescending(news => news.CreatedDate)
            .ToList();
            return View(data);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            var result = _newsService.Delete(id);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"News deleted: {id}", currentUserId);
            }
                return RedirectToAction(nameof(NewsList));
        }
    }
}
