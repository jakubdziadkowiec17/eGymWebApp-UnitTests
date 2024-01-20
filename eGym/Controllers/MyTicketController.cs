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
    public class MyTicketController : Controller
    {
        private readonly IMyTicketService _myTicketService;
        private readonly IGymService _gymService;
        private readonly ITicketService _ticketService;
        private UserManager<UserModel> userManager;
        private readonly IPaymentService _paymentService;
        public MyTicketController(IMyTicketService myTicketService, IGymService gymService, ITicketService ticketService, UserManager<UserModel> userManager, IPaymentService paymentService)
        {
            _myTicketService = myTicketService;
            _gymService = gymService;
            _ticketService = ticketService;
            this.userManager = userManager;
            _paymentService = paymentService;
        }


        [Authorize(Roles = "employee, client")]
        public IActionResult Add()
        {
            var data = this._gymService.List().ToList();
            var data1 = this._ticketService.List().ToList();

            var ticketList = data1.Select(ticket => new
            {
                Id = ticket.Id,
                Label = $"{ticket.TicketName} - {(ticket.ReducedTicket  ? "Discount ticket" : "Regular ticket")} - {(ticket.NumberOfDays != null ? ticket.NumberOfDays.Value.ToString() : "0")} days - {ticket.Price} zł"
            }).ToList();

            if(User.IsInRole("client"))
            {
                ViewBag.GymList = new SelectList(data, "Id", "GymName");
            }
            ViewBag.TicketList = new SelectList(ticketList, "Id", "Label");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "employee, client")]
        public async Task<IActionResult> AddAsync(MyTicketModel model)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await userManager.GetUserAsync(User);
            if(User.IsInRole("employee"))
            {
                model.GymId = currentUser.GymId;
            }
            var data = this._gymService.GetById(model.GymId);
            var data1 = this._ticketService.GetById(model.TicketId);
            var Client = await userManager.IsInRoleAsync(currentUser, "client");

            model.GymName = data.GymName;
            model.TicketName = data1.TicketName;
            model.ReducedTicket = data1.ReducedTicket;
            model.Price = data1.Price;
            model.NumberOfDays=data1.NumberOfDays;

            if (model.TicketStatus != TicketStatus.NotPaid && model.TicketStatus != TicketStatus.Paid && !Client)
            {
                if(model.NumberOfDays==null)
                {
                    model.ExpirationDate = DateTime.Now;
                }
                if(model.NumberOfDays != null)
                {
                    model.ExpirationDate = DateTime.Now.AddDays((double)model.NumberOfDays);//przypisana ilosc dni do konca
                }
            }

            if (Client)
            {
                model.TicketStatus = TicketStatus.NotPaid;
                model.UserId = currentUser.Id;
                model.Name = currentUser.Name;
                model.LastName = currentUser.LastName;
                model.Address = currentUser.Address;
            }

            if(model.TicketStatus == TicketStatus.Paid || model.TicketStatus == TicketStatus.Active) 
            { 
                model.PaymentDate = DateTime.Now;
            }

            var result = await _myTicketService.Add(model);
            if (result)
            {
                var logModel = new LogModel($"MyTicket added: {model.Id}", currentUserId);
                if (model.TicketStatus == TicketStatus.Paid || model.TicketStatus == TicketStatus.Active)
                {
                    var model2 = new PaymentModel();
                    model2.GymId = model.GymId;
                    model2.Deposit = true;
                    model2.Sum = model.Price;
                    model2.Source = "MyTickets";
                    model2.SourceId = model.Id.ToString();
                    model2.PaymentDate = (DateTime)model.PaymentDate;
                    model2.UserEnteringThePayment = currentUserId;
                    var result2 = _paymentService.Add(model2);
                    if (result2)
                    {
                        if (model.TicketStatus == TicketStatus.Paid)
                        {
                            var logModel2 = new LogModel($"MyTicket marked as paid: {model.Id}", model2.UserEnteringThePayment);
                        }
                        if (model.TicketStatus == TicketStatus.Active)
                        {
                            var logModel3 = new LogModel($"MyTicket marked as active: {model.Id}", model2.UserEnteringThePayment);
                        }
                        var logModel4 = new LogModel($"Payment added: {model2.Id}", model2.UserEnteringThePayment);
                    }
                    else
                    {
                        return View(model);
                    }
                }
                return RedirectToAction("MyTicketList");
            }
            else
            {
                return View(model);
            }
        }
        [Authorize(Roles = "admin, employee, client")]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var currentUser = await userManager.GetUserAsync(User);
            var Admin = await userManager.IsInRoleAsync(currentUser, "admin");
            var Employee = await userManager.IsInRoleAsync(currentUser, "employee");
            var data = _myTicketService.GetById(id);
            if (data != null && (data.UserId==currentUser.Id || Admin == true || Employee == true))
            {
                return View(data);
            }
            return NotFound();
        }
        [Authorize(Roles = "admin, employee, client")]
        public async Task<IActionResult> MyTicketListAsync(int id)
        {
            var currentUser = await userManager.GetUserAsync(User);
            var Client = await userManager.IsInRoleAsync(currentUser, "client");
            ViewBag.CurrentUserId =currentUser.Id;
            ViewBag.CurrentUserGymId = currentUser.GymId;
            var data = this._myTicketService.List().ToList();
            ViewBag.IdGym = currentUser.GymId;
            if (data != null)
            {
                if (Client)
                {
                    data = data.Where(t => t.UserId == currentUser.Id).ToList();
                }
                else if (id != 0)
                {
                    data = data.Where(t => t.GymId == id && t.ExpirationDate>=DateTime.Now).ToList();
                    ViewBag.IdGym = id;
                }
            }

            return View(data);
        }
        [Authorize(Roles = "admin, employee, client")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var currentUser = await userManager.GetUserAsync(User);
            var Admin = await userManager.IsInRoleAsync(currentUser, "admin");
            var Employee = await userManager.IsInRoleAsync(currentUser, "employee");
            var data = _myTicketService.GetById(id);
            if (Employee && currentUser.GymId != data.GymId)
            {
                return NotFound();
            }
            if (data != null)
            {
                if (data.TicketStatus == TicketStatus.NotPaid && (data.UserId == currentUser.Id || Admin == true || Employee == true))
                {
                    var result = _myTicketService.Delete(id);
                    if (result)
                    {
                        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var logModel = new LogModel($"MyTicket deleted: {id}", currentUserId);
                    }
                }
            }
            return RedirectToAction("MyTicketList");
        }
        [Authorize(Roles = "employee")]
        public async Task<IActionResult> PayAsync(int id)
        {
            var currentUser = await userManager.GetUserAsync(User);
            var Employee = await userManager.IsInRoleAsync(currentUser, "employee");
            var data = _myTicketService.GetById(id);
            if (Employee && currentUser.GymId != data.GymId)
            {
                return NotFound();
            }

            if (data != null)
            {
                if (data.TicketStatus == TicketStatus.NotPaid && (Employee == true))
                {
                    data.TicketStatus = TicketStatus.Paid;
                    data.PaymentDate = DateTime.Now;
                    var result = await _myTicketService.Update(data);
                    if (result)
                    {
                        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var logModel = new LogModel($"MyTicket marked as paid: {data.Id}", currentUserId);

                        var model2 = new PaymentModel();
                        model2.GymId = data.GymId;
                        model2.Deposit = true;
                        model2.Sum = data.Price;
                        model2.Source = "MyTickets";
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
            }
            return RedirectToAction("MyTicketList");
        }
        [Authorize(Roles = "employee, client")]
        public async Task<IActionResult> ActivateAsync(int id)
        {
            var currentUser = await userManager.GetUserAsync(User);
            var Employee = await userManager.IsInRoleAsync(currentUser, "employee");
            var data = _myTicketService.GetById(id);
            if (Employee && currentUser.GymId != data.GymId)
            {
                return NotFound();
            }

            if (data != null)
            {
                if (data.TicketStatus == TicketStatus.Paid && (data.UserId==currentUser.Id || Employee==true))
                {
                    data.TicketStatus = TicketStatus.Active;
                    if (data.NumberOfDays == null)
                    {
                        data.ExpirationDate = DateTime.Now;
                    }
                    else
                    {
                        data.ExpirationDate = DateTime.Now.AddDays((double)data.NumberOfDays);
                    }
                    var result = await _myTicketService.Update(data);
                    if (result)
                    {
                        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var logModel = new LogModel($"MyTicket marked as active: {data.Id}", currentUserId);
                    }
                }
            }
            return RedirectToAction("MyTicketList");
        }
    }
}