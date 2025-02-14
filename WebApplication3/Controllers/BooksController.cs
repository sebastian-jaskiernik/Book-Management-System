using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var books = _context.Books.Include(b => b.Author).Include(b => b.Publisher);
            return View(await books.ToListAsync());
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["Authors"] = new SelectList(_context.Authors, "AuthorId", "Name");
            ViewData["Publishers"] = new SelectList(_context.Publishers, "PublisherId", "Name");
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("Błędy walidacji (Book): " + string.Join(", ", errors));

                ViewData["Authors"] = new SelectList(_context.Authors, "AuthorId", "Name", book.AuthorId);
                ViewData["Publishers"] = new SelectList(_context.Publishers, "PublisherId", "Name", book.PublisherId);
                return View(book);
            }

            Console.WriteLine($"Dodaję książkę: {book.Title} (AutorID: {book.AuthorId}, WydawcaID: {book.PublisherId})");

            _context.Add(book);
            await _context.SaveChangesAsync();
            Console.WriteLine("Książka zapisana!");

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            ViewData["Authors"] = new SelectList(_context.Authors, "AuthorId", "Name", book.AuthorId);
            ViewData["Publishers"] = new SelectList(_context.Publishers, "PublisherId", "Name", book.PublisherId);

            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Book book)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Authors"] = new SelectList(_context.Authors, "AuthorId", "Name", book.AuthorId);
                ViewData["Publishers"] = new SelectList(_context.Publishers, "PublisherId", "Name", book.PublisherId);
                return View(book);
            }

            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(e => e.BookId == book.BookId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
