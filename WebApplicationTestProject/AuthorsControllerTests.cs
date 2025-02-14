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
    public class AuthorsControllerTests
    {
        private ApplicationDbContext _context;
        private AuthorsController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _context.Authors.AddRange(new List<Author>
            {
                new Author { AuthorId = 1, Name = "Author One" },
                new Author { AuthorId = 2, Name = "Author Two" }
            });
            _context.SaveChanges();

            _controller = new AuthorsController(_context);
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
        public async Task Index_Returns_View_With_Authors()
        {
            var result = await _controller.Index() as ViewResult;
            var model = result.Model as List<Author>;

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
            var newAuthor = new Author { AuthorId = 3, Name = "New Author" };
            var result = await _controller.Create(newAuthor) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_context.Authors.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task Create_Post_Returns_View_When_Invalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var author = new Author();
            var result = await _controller.Create(author) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(author));
        }

        [Test]
        public async Task Delete_Returns_NotFound_When_Id_Is_Null()
        {
            var result = await _controller.Delete(null) as NotFoundResult;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Delete_Returns_View_With_Author()
        {
            var result = await _controller.Delete(1) as ViewResult;
            var model = result.Model as Author;

            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.AuthorId, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteConfirmed_Removes_Author_And_Redirects()
        {
            var result = await _controller.DeleteConfirmed(1) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_context.Authors.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task Update_Returns_NotFound_When_Id_Is_Null()
        {
            var result = await _controller.Update((int?)null) as NotFoundResult;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Update_Returns_View_With_Author()
        {
            var result = await _controller.Update(1) as ViewResult;
            var model = result.Model as Author;

            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.AuthorId, Is.EqualTo(1));
        }

        [Test]
        public async Task Update_Post_Updates_Author_And_Redirects()
        {
            var author = _context.Authors.Find(1);
            author.Name = "Updated Name";

            var result = await _controller.Update(author) as RedirectToActionResult;
            var updatedAuthor = _context.Authors.Find(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(updatedAuthor.Name, Is.EqualTo("Updated Name"));
        }
    }
}
