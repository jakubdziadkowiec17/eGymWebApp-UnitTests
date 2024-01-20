using eGym.Models;
using eGym.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace eGym_UnitTests.Repositories
{
    public class ClassesServiceUnitTest
    {
        [Fact]
        public void Add_ReturnsTrue_ModelIsValid()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "Add_ReturnsTrue_ModelIsValid").Options;
            var context = new DatabaseContext(dbContext);
            var service = new ClassesService(context);

            var classes = service.Add(new ClassesModel { Id = 1, Name = "Classes 1", NumberOfPeople = 1, StartDate = DateTime.Now, EndDate = DateTime.Now, EmployeeId = "abcd", Price=1, GymId=1 });

            Assert.Equal(1, context.Classes.Count());
        }

        [Fact]
        public void Delete_ReturnsTrue_WhenClassesExists()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "Delete_ReturnsTrue_WhenClassesExists").Options;
            var context = new DatabaseContext(dbContext);
            var service = new ClassesService(context);
            var classes = new ClassesModel { Id = 1, Name = "Classes 1", NumberOfPeople = 1, StartDate = DateTime.Now, EndDate = DateTime.Now, EmployeeId = "abcd", Price = 1, GymId = 1 };
            context.Classes.Add(classes);
            context.SaveChanges();

            var result = service.Delete(classes.Id);

            Assert.True(result);
            Assert.Equal(0, context.Classes.Count());
        }

        [Fact]
        public void GetById_ReturnsCorrectModel_WhenClassesExists()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "GetById_ReturnsCorrectModel_WhenClassesExists").Options;
            var context = new DatabaseContext(dbContext);
            var service = new ClassesService(context);
            var classes = new ClassesModel { Id = 1, Name = "Classes 1", NumberOfPeople = 1, StartDate = DateTime.Now, EndDate = DateTime.Now, EmployeeId = "abcd", Price = 1, GymId = 1 };
            context.Classes.Add(classes);
            context.SaveChanges();

            var result = service.GetById(classes.Id);

            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void List_ReturnsList()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "List_ReturnsList").Options;
            var context = new DatabaseContext(dbContext);
            var service = new ClassesService(context);
            context.Classes.Add(new ClassesModel { Id = 1, Name = "Classes 1", NumberOfPeople = 1, StartDate = DateTime.Now, EndDate = DateTime.Now, EmployeeId = "abcd", Price = 1, GymId = 1 });
            context.Classes.Add(new ClassesModel { Id = 2, Name = "Classes 2", NumberOfPeople = 2, StartDate = DateTime.Now, EndDate = DateTime.Now, EmployeeId = "abcd", Price = 2, GymId = 2 });
            context.SaveChanges();

            var result = service.List().ToList();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void Update_ReturnsTrue_WhenModelIsValid()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "Update_ReturnsTrue_WhenModelIsValid").Options;
            var context = new DatabaseContext(dbContext);
            var service = new ClassesService(context);
            var classes = new ClassesModel { Id = 1, Name = "Classes 1", NumberOfPeople = 1, StartDate = DateTime.Now, EndDate = DateTime.Now, EmployeeId = "abcd", Price = 1, GymId = 1 };
            context.Classes.Add(classes);
            context.SaveChanges();

            var result = service.Update(classes);

            Assert.True(result);
        }
    }
}