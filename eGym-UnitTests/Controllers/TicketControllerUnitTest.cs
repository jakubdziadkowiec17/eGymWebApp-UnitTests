using System.Security.Claims;
using eGym.Controllers;
using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace eGym_UnitTests.Controllers
{
    public class TaskServiceUnitTest
    {
        [Fact]
        public void Add_Get_ReturnsView()
        {
            var serviceMock = new Mock<ITicketService>();
            var controller = new TicketController(serviceMock.Object);

            var result = controller.Add();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public void Add_Post_ValidModel_ReturnsRedirectToAction()
        {
            var serviceMock = new Mock<ITicketService>();
            serviceMock.Setup(service => service.Add(It.IsAny<TicketModel>())).Returns(true);
            var controller = new TicketController(serviceMock.Object);
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "abcd")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var model = new TicketModel { Id = 2, TicketName = "abc", NumberOfDays = 30, ReducedTicket = true, Price = 30 };

            var result = controller.Add(model);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("TicketList", redirectToActionResult.ActionName);
        }

        [Fact]
        public void Edit_Get_ValidModel_ReturnsViewWithModel()
        {
            var serviceMock = new Mock<ITicketService>();
            var controller = new TicketController(serviceMock.Object);

            var ticketModel = new TicketModel { Id = 2, TicketName = "abc", NumberOfDays = 30, ReducedTicket = true, Price = 30 };
            serviceMock.Setup(repo => repo.GetById(2)).Returns(ticketModel);

            var result = controller.Edit(2);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TicketModel>(viewResult.Model);
            Assert.Equal(ticketModel, model);
        }

        [Fact]
        public void TicketList_ReturnsView()
        {
            var serviceMock = new Mock<ITicketService>();
            serviceMock.Setup(repo => repo.List()).Returns(Enumerable.Empty<TicketModel>().AsQueryable());
            var controller = new TicketController(serviceMock.Object);

            var result = controller.TicketList();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<TicketModel>>(viewResult.ViewData.Model);
            Assert.Empty(model);
        }

        [Fact]
        public void Delete_ValidModel_ReturnsRedirectToAction()
        {
            var serviceMock = new Mock<ITicketService>();
            var controller = new TicketController(serviceMock.Object);
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "abcd")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = controller.Delete(2);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("TicketList", redirectToActionResult.ActionName);
        }
    }
}