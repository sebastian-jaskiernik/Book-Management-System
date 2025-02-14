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
    public class BooksControllerTests
    {
        private ApplicationDbContext _context;
        private BooksController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _context.Authors.Add(new Author { AuthorId = 1, Name = "Author One" });
            _context.Publishers.Add(new Publisher { PublisherId = 1, Name = "Publisher One" });
            _context.Books.AddRange(new List<Book>
            {
                new Book { BookId = 1, Title = "Book One", AuthorId = 1, PublisherId = 1 },
                new Book { BookId = 2, Title = "Book Two", AuthorId = 1, PublisherId = 1 }
            });
            _context.SaveChanges();

            _controller = new BooksController(_context);
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
        public async Task Index_Returns_View_With_Books()
        {
            var result = await _controller.Index() as ViewResult;
            var model = result.Model as List<Book>;

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
            var newBook = new Book { BookId = 3, Title = "New Book", AuthorId = 1, PublisherId = 1 };
            var result = await _controller.Create(newBook) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_context.Books.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task Create_Post_Returns_View_When_Invalid()
        {
            _controller.ModelState.AddModelError("Title", "Required");
            var book = new Book();
            var result = await _controller.Create(book) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(book));
        }

        [Test]
        public async Task Delete_Returns_NotFound_When_Id_Is_Null()
        {
            var result = await _controller.Delete(null) as NotFoundResult;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Delete_Returns_View_With_Book()
        {
            var result = await _controller.Delete(1) as ViewResult;
            var model = result.Model as Book;

            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.BookId, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteConfirmed_Removes_Book_And_Redirects()
        {
            var result = await _controller.DeleteConfirmed(1) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(_context.Books.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task Update_Returns_NotFound_When_Id_Is_Null()
        {
            var result = await _controller.Update((int?)null) as NotFoundResult;
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Update_Returns_View_With_Book()
        {
            var result = await _controller.Update(1) as ViewResult;
            var model = result.Model as Book;

            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.BookId, Is.EqualTo(1));
        }

        [Test]
        public async Task Update_Post_Updates_Book_And_Redirects()
        {
            var book = _context.Books.Find(1);
            book.Title = "Updated Title";

            var result = await _controller.Update(book) as RedirectToActionResult;
            var updatedBook = _context.Books.Find(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(updatedBook.Title, Is.EqualTo("Updated Title"));
        }
    }
}
