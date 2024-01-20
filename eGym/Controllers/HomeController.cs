using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace eGym.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGymService _gymService;
        private readonly IAdService _adService;
        private readonly INewsService _newsService;
        private UserManager<UserModel> userManager;
        public HomeController(ILogger<HomeController> logger, IGymService gymService, IAdService adService, INewsService newsService, UserManager<UserModel> userManager)
        {
            _logger = logger;
            _gymService = gymService;
            _adService = adService;
            _newsService = newsService;
            this.userManager = userManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var data = _gymService.List().ToList();
            var data1 = _adService.List().ToList();
            var data2 = _newsService.List()
            .Where(news => news.Active)
            .OrderByDescending(news => news.CreatedDate)
            .ToList();

            var model=new IndexModel
            {
                Gym = data,
                Ad = data1,
                NewsItem = data2
            };

            var currentUserId1 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data3 = await userManager.FindByIdAsync(currentUserId1);
            if (data3 != null)
            {
                ViewBag.Gym = data3.GymId;
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}