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
    public class AdControllerUnitTest
    {
        [Fact]
        public void Add_ReturnsView_WhenUserIsAdmin()
        {
            var serviceMock = new Mock<IAdService>();
            var environmentMock = new Mock<IWebHostEnvironment>();
            var controller = new AdController(serviceMock.Object, environmentMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "admin") })) }
            };

            var result = controller.Add() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddPost_RedirectsToAdList_ModelIsValid()
        {
            var serviceMock = new Mock<IAdService>();
            serviceMock.Setup(x => x.Add(It.IsAny<AdModel>())).ReturnsAsync(true);
            var environmentMock = new Mock<IWebHostEnvironment>();
            environmentMock.Setup(x => x.WebRootPath).Returns("file");
            var controller = new AdController(serviceMock.Object, environmentMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, "userId"),
                new Claim(ClaimTypes.Role, "admin")
            }))
                }
            };

            var model = new AdModel { Title = "Ad 1", Image = new FormFile(Stream.Null, 0, 0, "Image", "Ad 1.png"), Id = 2, CreatedDate = DateTime.Now, UserId = "abcd", ImagePath = "Ad 1.png" };

            var result = await controller.Add(model) as RedirectToActionResult;

            Assert.Equal("AdList", result.ActionName);
        }

        [Fact]
        public void AdList_ReturnsView_WhenUserIsAdmin()
        {
            var serviceMock = new Mock<IAdService>();
            var environmentMock = new Mock<IWebHostEnvironment>();
            var controller = new AdController(serviceMock.Object, environmentMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "admin") })) }
            };

            var result = controller.AdList() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void Delete_RedirectsToAdList_WhenAdDeletedSuccessfully()
        {
            var serviceMock = new Mock<IAdService>();
            serviceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(new AdModel { Title = "Ad 1", Image = new FormFile(Stream.Null, 0, 0, "Image", "Ad 1.png"), Id = 2, CreatedDate = DateTime.Now, UserId = "abcd", ImagePath = "Ad 1.png" });
            serviceMock.Setup(x => x.Delete(It.IsAny<int>())).Returns(true);

            var environmentMock = new Mock<IWebHostEnvironment>();
            environmentMock.Setup(x => x.WebRootPath).Returns("file");
            var controller = new AdController(serviceMock.Object, environmentMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "userId"), new Claim(ClaimTypes.Role, "admin") })) }
            };

            var result = controller.Delete(1) as RedirectToActionResult;

            Assert.Equal("AdList", result.ActionName);
        }
    }
}