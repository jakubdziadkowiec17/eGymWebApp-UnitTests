using eGym.Models;
using eGym.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace eGym_UnitTests.Repositories
{
    public class GymServiceUnitTest
    {
        [Fact]
        public void Add_ReturnsTrue_ModelIsValid()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "Add_ReturnsTrue_ModelIsValid").Options;
            var context = new DatabaseContext(dbContext);
            var service = new GymService(context);

            var gym = service.Add(new GymModel { Id = 1, GymName = "Gym 1", Locality = "abcd", CreatedDate = DateTime.Now, OpeningHours = "abcd", Map="abcd"});

            Assert.True(gym);
            Assert.Equal(1, context.Gym.Count());
        }

        [Fact]
        public void Delete_ReturnsTrue_WhenGymExists()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "Delete_ReturnsTrue_WhenGymExists").Options;
            var context = new DatabaseContext(dbContext);
            var service = new GymService(context);
            var gym = new GymModel { Id = 1, GymName = "Gym 1", Locality = "abcd", CreatedDate = DateTime.Now, OpeningHours = "abcd", Map = "abcd" };
            context.Gym.Add(gym);
            context.SaveChanges();

            var result = service.Delete(gym.Id);

            Assert.True(result);
            Assert.Equal(0, context.Gym.Count());
        }

        [Fact]
        public void GetById_ReturnsCorrectModel_WhenGymExists()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "GetById_ReturnsCorrectModel_WhenGymExists").Options;
            var context = new DatabaseContext(dbContext);
            var service = new GymService(context);
            var gym = new GymModel { Id = 1, GymName = "Gym 1", Locality = "abcd", CreatedDate = DateTime.Now, OpeningHours = "abcd", Map = "abcd" };
            context.Gym.Add(gym);
            context.SaveChanges();

            var result = service.GetById(gym.Id);

            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void List_ReturnsList()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "List_ReturnsList").Options;
            var context = new DatabaseContext(dbContext);
            var service = new GymService(context);
            context.Gym.Add(new GymModel { Id = 1, GymName = "Gym 1", Locality = "abcd", CreatedDate = DateTime.Now, OpeningHours = "abcd", Map = "abcd" });
            context.Gym.Add(new GymModel { Id = 2, GymName = "Gym 2", Locality = "abcd", CreatedDate = DateTime.Now, OpeningHours = "abcd", Map = "abcd" });
            context.SaveChanges();

            var result = service.List().ToList();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void Update_ReturnsTrue_WhenModelIsValid()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "Update_ReturnsTrue_WhenModelIsValid").Options;
            var context = new DatabaseContext(dbContext);
            var service = new GymService(context);
            var gym = new GymModel { Id = 1, GymName = "Gym 1", Locality = "abcd", CreatedDate = DateTime.Now, OpeningHours = "abcd", Map = "abcd" };
            context.Gym.Add(gym);
            context.SaveChanges();

            var result = service.Update(gym);

            Assert.True(result);
        }
    }
}