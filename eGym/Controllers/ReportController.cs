using Microsoft.AspNetCore.Mvc;
using eGym.Repositories;
using eGym.Models;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using System;

namespace eGym.Controllers
{
    public class ReportController : Controller
    {
        private readonly IMyTicketService _myTicketService;
        private readonly ITicketService _ticketService;
        private readonly IGymService _gymService;
        private readonly IOpinionService _opinionService;
        private readonly IPaymentService _paymentService;
        public ReportController(IMyTicketService myTicketService, ITicketService ticketService, IGymService gymService, IOpinionService opinionService, IPaymentService paymentService)
        {
            _myTicketService = myTicketService;
            _ticketService = ticketService;
            _gymService = gymService;
            _opinionService = opinionService;
            _paymentService = paymentService;
        }

        [Authorize(Roles = "admin")]
        public IActionResult Generate()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> TicketsReport(ReportModel model)
        {
            var data = this._paymentService.List().Where(t => t.Source == "MyTickets").ToList();
            var data1 = this._gymService.List().ToList();
            if (model.StartDate.HasValue)
            {
                data = data.Where(t => t.PaymentDate >= model.StartDate).ToList();
            }
            if (model.EndDate.HasValue)
            {
                data = data.Where(t => t.PaymentDate <= model.EndDate).ToList();
            }

            var tickets = data1.Select(gym => new
            {
                Gym = gym.GymName + " " + gym.Locality,
                Count = data.Count(t => t.GymId == gym.Id),
                SumOfPrice = data.Where(t => t.GymId == gym.Id).Sum(t => t.Sum)
            });
            ViewBag.Start = model.StartDate;
            ViewBag.End = model.EndDate;

            return View(tickets);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ClassesReport(ReportModel model)
        {
            var data = this._paymentService.List().Where(t => t.Source == "MyClasses").ToList();
            var data1 = this._gymService.List().ToList();
            if (model.StartDate.HasValue)
            {
                data = data.Where(t => t.PaymentDate >= model.StartDate).ToList();
            }
            if (model.EndDate.HasValue)
            {
                data = data.Where(t => t.PaymentDate <= model.EndDate).ToList();
            }

            var tickets = data1.Select(gym => new
            {
                Gym = gym.GymName + " " + gym.Locality,
                Count = data.Count(t => t.GymId == gym.Id),
                SumOfPrice = data.Where(t => t.GymId == gym.Id).Sum(t => t.Sum)
            });
            ViewBag.Start = model.StartDate;
            ViewBag.End = model.EndDate;

            return View(tickets);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PaymentsReport(ReportModel model)
        {
            var data = this._paymentService.List().ToList();
            var data1 = this._gymService.List().ToList();
            if (model.StartDate.HasValue)
            {
                data = data.Where(t => t.PaymentDate >= model.StartDate).ToList();
            }
            if (model.EndDate.HasValue)
            {
                data = data.Where(t => t.PaymentDate <= model.EndDate).ToList();
            }
            var tickets = data1.Select(gym => new
            {
                Gym = gym.GymName + " " + gym.Locality,
                SumOfPrice = CalculateSum(data, gym.Id)
            });
            ViewBag.Start = model.StartDate;
            ViewBag.End = model.EndDate;

            return View(tickets);
        }

        private int CalculateSum(List<PaymentModel> payments, int gymId)
        {
            int sum = 0;
            foreach (var payment in payments.Where(t => t.GymId == gymId))
            {
                if (payment.Deposit)
                {
                    sum += payment.Sum;
                }
                else
                {
                    sum -= payment.Sum;
                }
            }
            return sum;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> OpinionsReport(ReportModel model)
        {
            var data = this._opinionService.List().ToList();
            var data1 = this._gymService.List().ToList();
            if (model.StartDate.HasValue)
            {
                data = data.Where(t => t.OpinionDate >= model.StartDate).ToList();
            }
            if (model.EndDate.HasValue)
            {
                data = data.Where(t => t.OpinionDate <= model.EndDate).ToList();
            }

            var opinions = data1.Select(gym => new
            {
                Gym = gym.GymName + " " + gym.Locality,
                Count = data.Count(t => t.GymId == gym.Id),
                Avg = data.Where(t => t.GymId == gym.Id).Select(t => (decimal?)t.Rating).Average()
            });
            ViewBag.Start = model.StartDate;
            ViewBag.End = model.EndDate;

            return View(opinions);
        }
    }
}
