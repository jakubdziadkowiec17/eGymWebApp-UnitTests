using eGym.Models;
using eGym.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace eGym_UnitTests.Repositories
{
    public class TaskServiceUnitTest
    {
        [Fact]
        public void Add_ReturnsTrue_ModelIsValid()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "Add_ReturnsTrue_ModelIsValid").Options;
            var context = new DatabaseContext(dbContext);
            var service = new TaskService(context);

            var task = service.Add(new TaskModel { Id = 1, Title = "Task 1", Content = "abcd", CreatedDate = DateTime.Now, UserId = "abcd", EndDate = DateTime.Now, TaskStatus = eGym.Models.TaskStatus.Completed });

            Assert.True(task);
            Assert.Equal(1, context.Task.Count());
        }

        [Fact]
        public void Delete_ReturnsTrue_WhenTaskExists()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "Delete_ReturnsTrue_WhenTaskExists").Options;
            var context = new DatabaseContext(dbContext);
            var service = new TaskService(context);
            var task = new TaskModel { Id = 1, Title = "Task 1", Content = "abcd", CreatedDate = DateTime.Now, UserId = "abcd", EndDate = DateTime.Now, TaskStatus = eGym.Models.TaskStatus.Completed };
            context.Task.Add(task);
            context.SaveChanges();

            var result = service.Delete(task.Id);

            Assert.True(result);
            Assert.Equal(0, context.Task.Count());
        }

        [Fact]
        public void GetById_ReturnsCorrectModel_WhenTaskExists()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "GetById_ReturnsCorrectModel_WhenTaskExists").Options;
            var context = new DatabaseContext(dbContext);
            var service = new TaskService(context);
            var task = new TaskModel { Id = 1, Title = "Task 1", Content = "abcd", CreatedDate = DateTime.Now, UserId = "abcd", EndDate = DateTime.Now, TaskStatus = eGym.Models.TaskStatus.Completed };
            context.Task.Add(task);
            context.SaveChanges();

            var result = service.GetById(task.Id);

            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void List_ReturnsList()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "List_ReturnsList").Options;
            var context = new DatabaseContext(dbContext);
            var service = new TaskService(context);
            context.Task.Add(new TaskModel { Id = 1, Title = "Task 1", Content = "abcd", CreatedDate = DateTime.Now, UserId = "abcd", EndDate = DateTime.Now, TaskStatus = eGym.Models.TaskStatus.Completed });
            context.Task.Add(new TaskModel { Id = 2, Title = "Task 2", Content = "abcd", CreatedDate = DateTime.Now, UserId = "abcd", EndDate = DateTime.Now, TaskStatus = eGym.Models.TaskStatus.Completed });
            context.SaveChanges();

            var result = service.List().ToList();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void Update_ReturnsTrue_WhenModelIsValid()
        {
            var dbContext = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "Update_ReturnsTrue_WhenModelIsValid").Options;
            var context = new DatabaseContext(dbContext);
            var service = new TaskService(context);
            var task = new TaskModel { Id = 1, Title = "Task 1", Content = "abcd", CreatedDate = DateTime.Now, UserId = "abcd", EndDate = DateTime.Now, TaskStatus = eGym.Models.TaskStatus.Completed };
            context.Task.Add(task);
            context.SaveChanges();

            var result = service.Update(task);

            Assert.True(result);
        }
    }
}