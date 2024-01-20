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
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IGymService _gymService;
        private UserManager<UserModel> userManager;
        public PaymentController(IPaymentService paymentService, IGymService gymService, UserManager<UserModel> userManager)
        {
            _paymentService = paymentService;
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
        public IActionResult Add(PaymentModel model)
        {
            model.PaymentDate = DateTime.Now;
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            model.UserEnteringThePayment = currentUserId;
            model.Source = "None";

            var result = _paymentService.Add(model);
            if (result)
            {
                var logModel = new LogModel($"Payment added: {model.Id}", model.UserEnteringThePayment);
                return RedirectToAction("PaymentList", new { id = model.GymId });
            }
            else
            {
                return View(model);
            }
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PaymentListAsync(int id)
        {
            var data = this._paymentService.List().Where(payment => payment.GymId == id).OrderByDescending(payment => payment.Id).ToList();

            if (data.Count==0 && id == 0)
            {
                data = this._paymentService.List().OrderByDescending(payment => payment.Id).ToList();
                ViewBag.GymName = "";
            }
            else
            {
                var data1 = _gymService.GetById(id);
                ViewBag.GymName = "- " + data1.GymName;
            }

            int sum1 = data.Where(payment => payment.Deposit == true).Sum(payment => payment.Sum);
            int sum2 = data.Where(payment => payment.Deposit == false).Sum(payment => payment.Sum);
            ViewBag.Sum = sum1 - sum2;

            var data2 = this._gymService.List().ToList();
            ViewBag.Gym = new SelectList(data2, "Id", "GymName");

            var userList = await userManager.Users
            .Select(u => new { Id = u.Id, FullName = u.Name + " " + u.LastName })
            .ToListAsync();

            ViewBag.Users = new SelectList(userList, "Id", "FullName");

            return View(data);
        }
    }
}