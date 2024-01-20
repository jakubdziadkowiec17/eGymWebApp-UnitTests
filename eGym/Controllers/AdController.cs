using Microsoft.AspNetCore.Mvc;
using eGym.Repositories;
using eGym.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace eGym.Controllers
{
    public class AdController : Controller
    {
        private readonly IAdService _adService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdController(IAdService adService, IWebHostEnvironment webHostEnvironment)
        {
            _adService = adService;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = "admin")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Add(AdModel model)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath + "/Ad/", model.Title + Path.GetExtension(model.Image.FileName));
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await model.Image.CopyToAsync(fileStream);
            }
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            model.UserId = currentUserId;
            model.ImagePath = model.Title + Path.GetExtension(model.Image.FileName);
            var result = await _adService.Add(model);
            if (result)
            {
                return RedirectToAction(nameof(AdList));
            }
            return RedirectToAction(nameof(AdList));
        }

        [Authorize(Roles = "admin")]
        public IActionResult AdList()
        {
            var data = this._adService.List().ToList();
            return View(data);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            var temp = _adService.GetById(id);
            string path = Path.Combine(_webHostEnvironment.WebRootPath + "/Ad/", temp.ImagePath);
            System.IO.File.Delete(path);

            var result = _adService.Delete(id);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Advertisement deleted: {id}", currentUserId);
            }
            return RedirectToAction(nameof(AdList));
        }
    }
}
