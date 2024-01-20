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
    public class ReportControllerUnitTest
    {
        [Fact]
        public void Generate_ReturnsView_Admin()
        {
            var service1Mock = new Mock<IMyTicketService>();
            var service2Mock = new Mock<ITicketService>();
            var service3Mock = new Mock<IGymService>();
            var service4Mock = new Mock<IOpinionService>();
            var service5Mock = new Mock<IPaymentService>();
            var controller = new ReportController(
                service1Mock.Object,
                service2Mock.Object,
                service3Mock.Object,
                service4Mock.Object,
                service5Mock.Object
            );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "admin") })) }
            };

            var result = controller.Generate() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task TicketsReport_ReturnsView_WithData()
        {
            var service1Mock = new Mock<IMyTicketService>();
            var service2Mock = new Mock<ITicketService>();
            var service3Mock = new Mock<IGymService>();
            var service4Mock = new Mock<IOpinionService>();
            var service5Mock = new Mock<IPaymentService>();
            var controller = new ReportController(
                service1Mock.Object,
                service2Mock.Object,
                service3Mock.Object,
                service4Mock.Object,
                service5Mock.Object
            );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "admin") })) }
            };
            var model = new ReportModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };

            var result = await controller.TicketsReport(model) as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ClassesReport_ReturnsView_WithData()
        {
            var service1Mock = new Mock<IMyTicketService>();
            var service2Mock = new Mock<ITicketService>();
            var service3Mock = new Mock<IGymService>();
            var service4Mock = new Mock<IOpinionService>();
            var service5Mock = new Mock<IPaymentService>();
            var controller = new ReportController(
                service1Mock.Object,
                service2Mock.Object,
                service3Mock.Object,
                service4Mock.Object,
                service5Mock.Object
            );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "admin") })) }
            };
            var model = new ReportModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };

            var result = await controller.ClassesReport(model) as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task PaymentsReport_ReturnsView_WithData()
        {
            var service1Mock = new Mock<IMyTicketService>();
            var service2Mock = new Mock<ITicketService>();
            var service3Mock = new Mock<IGymService>();
            var service4Mock = new Mock<IOpinionService>();
            var service5Mock = new Mock<IPaymentService>();
            var controller = new ReportController(
                service1Mock.Object,
                service2Mock.Object,
                service3Mock.Object,
                service4Mock.Object,
                service5Mock.Object
            );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "admin") })) }
            };
            var model = new ReportModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };

            var result = await controller.PaymentsReport(model) as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task OpinionsReport_ReturnsView_WithData()
        {
            var service1Mock = new Mock<IMyTicketService>();
            var service2Mock = new Mock<ITicketService>();
            var service3Mock = new Mock<IGymService>();
            var service4Mock = new Mock<IOpinionService>();
            var service5Mock = new Mock<IPaymentService>();
            var controller = new ReportController(
                service1Mock.Object,
                service2Mock.Object,
                service3Mock.Object,
                service4Mock.Object,
                service5Mock.Object
            );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "admin") })) }
            };
            var model = new ReportModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };

            var result = await controller.OpinionsReport(model) as ViewResult;

            Assert.NotNull(result);
        }
    }
}