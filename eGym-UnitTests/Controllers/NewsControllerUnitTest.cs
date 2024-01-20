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
    public class NewsControllerUnitTest
    {
        [Fact]
        public void Add_Get_ReturnsView()
        {
            var serviceMock = new Mock<INewsService>();
            var controller = new NewsController(serviceMock.Object);

            var result = controller.Add();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void AddPost_ValidModel_ReturnsRedirectToAction()
        {
            var serviceMock = new Mock<INewsService>();
            serviceMock.Setup(service => service.Add(It.IsAny<NewsModel>())).Returns(true);
            var controller = new NewsController(serviceMock.Object);
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "abcd")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var model = new NewsModel { Id = 2, Title = "Test News", Content = "Lorem Ipsum", Active = true, CreatedDate = DateTime.Now, UserId = "abcd" };

            var result = controller.Add(model);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("NewsList", redirectToActionResult.ActionName);
        }

        [Fact]
        public void Edit_Get_ReturnsView()
        {
            var serviceMock = new Mock<INewsService>();
            serviceMock.Setup(repo => repo.GetById(It.IsAny<int>())).Returns(new NewsModel());
            var controller = new NewsController(serviceMock.Object);

            var result = controller.Edit(2);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Update_ValidModel_ReturnsRedirectToAction()
        {
            var serviceMock = new Mock<INewsService>();
            serviceMock.Setup(service => service.Update(It.IsAny<NewsModel>())).Returns(true);
            var controller = new NewsController(serviceMock.Object);
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "abcd")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var model = new NewsModel { Id = 2, Title = "Test News", Content = "Lorem Ipsum", Active = true, CreatedDate = DateTime.Now, UserId = "abcd" };

            var result = controller.Update(model);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("NewsList", redirectToActionResult.ActionName);
        }

        [Fact]
        public void Details_ValidModel_ReturnsView()
        {
            var serviceMock = new Mock<INewsService>();
            serviceMock.Setup(repo => repo.GetById(It.IsAny<int>())).Returns(new NewsModel());
            var controller = new NewsController(serviceMock.Object);

            var result = controller.Details(1);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void NewsList_ReturnsView()
        {
            var serviceMock = new Mock<INewsService>();
            serviceMock.Setup(repo => repo.List()).Returns(Enumerable.Empty<NewsModel>().AsQueryable());
            var controller = new NewsController(serviceMock.Object);

            var result = controller.NewsList();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<NewsModel>>(viewResult.ViewData.Model);
            Assert.Empty(model);
        }

        [Fact]
        public void Delete_ValidModel_ReturnsRedirectToAction()
        {
            var serviceMock = new Mock<INewsService>();
            var controller = new NewsController(serviceMock.Object);
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
            Assert.Equal("NewsList", redirectToActionResult.ActionName);
        }
    }
}