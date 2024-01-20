using Microsoft.AspNetCore.Mvc;
using eGym.Repositories;
using eGym.Models;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;

namespace eGym.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [Authorize(Roles = "admin")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Add(TicketModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = _ticketService.Add(model);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return RedirectToAction("TicketList");
            }
            else
            {
                return View(model);
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult Edit(int id)
        {
            var data = _ticketService.GetById(id);
            return View(data);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Update(TicketModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = _ticketService.Update(model);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Ticket updated: {model.Id}", currentUserId);
                return RedirectToAction(nameof(TicketList));
            }
            else
            {
                return View(model);
            }
        }
        public IActionResult TicketList()
        {
            var data = this._ticketService.List().ToList();
            return View(data);
        }

        [Authorize(Roles="admin")]
        public IActionResult Delete(int id)
        {
            var result = _ticketService.Delete(id);
            if (result)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var logModel = new LogModel($"Ticket deleted: {id}", currentUserId);
            }
                return RedirectToAction(nameof(TicketList));
        }
    }
}
