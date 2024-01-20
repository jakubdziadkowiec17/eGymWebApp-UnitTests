using eGym.Models;
using eGym.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace eGym_UnitTests.Repositories
{
    public class AdServiceUnitTest
    {
        [Fact]
        public void Add_ReturnsTrue_ModelIsValid()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "Add_ReturnsTrue_ModelIsValid").Options;
            var context = new DatabaseContext(dbContext);
            var service = new AdService(context);

            var ad = service.Add(new AdModel { Id = 1, Title = "Ad 1", ImagePath = "Ad 1", CreatedDate = DateTime.Now, UserId = "abcd" });

            Assert.Equal(1, context.Ad.Count());
        }

        [Fact]
        public void Delete_ReturnsTrue_WhenAdExists()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "Delete_ReturnsTrue_WhenAdExists").Options;
            var context = new DatabaseContext(dbContext);
            var service = new AdService(context);
            var ad = new AdModel { Id = 1, Title = "Ad 1", ImagePath = "Ad 1", CreatedDate = DateTime.Now, UserId = "abcd" };
            context.Ad.Add(ad);
            context.SaveChanges();

            var result = service.Delete(ad.Id);

            Assert.True(result);
            Assert.Equal(0, context.Ad.Count());
        }

        [Fact]
        public void GetById_ReturnsCorrectModel_WhenAdExists()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "GetById_ReturnsCorrectModel_WhenAdExists").Options;
            var context = new DatabaseContext(dbContext);
            var service = new AdService(context);
            var ad = new AdModel { Id = 1, Title = "Ad 1", ImagePath = "Ad 1", CreatedDate = DateTime.Now, UserId = "abcd" };
            context.Ad.Add(ad);
            context.SaveChanges();

            var result = service.GetById(ad.Id);

            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void List_ReturnsList()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "List_ReturnsList").Options;
            var context = new DatabaseContext(dbContext);
            var service = new AdService(context);

            context.Ad.Add(new AdModel { Id = 1, Title = "Ad 1", ImagePath = "Ad 1", CreatedDate = DateTime.Now, UserId = "abcd" });
            context.Ad.Add(new AdModel { Id = 2, Title = "Ad 2", ImagePath = "Ad 2", CreatedDate = DateTime.Now, UserId = "abcd" });
            context.SaveChanges();

            var result = service.List().ToList();

            Assert.Equal(2, result.Count);
        }
    }
}