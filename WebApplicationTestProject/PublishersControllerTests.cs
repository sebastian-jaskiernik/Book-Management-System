using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Controllers;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Tests
{
    [TestFixture]
    public class PublishersControllerTests
    {
        private ApplicationDbContext _context;
        private PublishersController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _context.Publishers.AddRange(new List<Publisher>
            {
                new Publisher { PublisherId = 1, Name = "Publisher One" },
                new Publisher { PublisherId = 2, Name = "Publisher Two" }
            });
            _context.SaveChanges();

            _controller = new PublishersController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            if (_controller is IDisposable disposableController)
            {
                disposableController.Dispose();
            }
        }

        [Test]
        public async Task Index_Returns_View_With_Publishers()
        {
            var result = await _controller.Index() as ViewResult;
            var model = result.Model as List<Publisher>;

            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(2));
        }

        [Test]
        public void Create_Returns_View()
        {
            var result = _controller.Create() as ViewResult;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Create_Post_Redirects_To_Index_When_Valid()
        {
            var newPublisher = new Publisher { PublisherId = 3, Name = "New Publisher" };
            var result = await _controller.Create(newPublisher) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_context.Publishers.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task Create_Post_Returns_View_When_Invalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var publisher = new Publisher();
            var result = await _controller.Create(publisher) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(publisher));
        }

        [Test]
        public async Task Delete_Returns_NotFound_When_Id_Is_Null()
        {
            var result = await _controller.Delete(null) as NotFoundResult;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Delete_Returns_View_With_Publisher()
        {
            var result = await _controller.Delete(1) as ViewResult;
            var model = result.Model as Publisher;

            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.PublisherId, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteConfirmed_Removes_Publisher_And_Redirects()
        {
            var result = await _controller.DeleteConfirmed(1) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_context.Publishers.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task Update_Returns_NotFound_When_Id_Is_Null()
        {
            var result = await _controller.Update((int?)null) as NotFoundResult;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Update_Returns_View_With_Publisher()
        {
            var result = await _controller.Update(1) as ViewResult;
            var model = result.Model as Publisher;

            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.PublisherId, Is.EqualTo(1));
        }

        [Test]
        public async Task Update_Post_Updates_Publisher_And_Redirects()
        {
            var publisher = _context.Publishers.Find(1);
            publisher.Name = "Updated Name";

            var result = await _controller.Update(publisher) as RedirectToActionResult;
            var updatedPublisher = _context.Publishers.Find(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(updatedPublisher.Name, Is.EqualTo("Updated Name"));
        }
    }
}
