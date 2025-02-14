using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class PublishersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PublishersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Publishers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Publishers.ToListAsync());
        }

        // GET: Publishers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Publishers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Publisher publisher)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("Błędy walidacji (Publisher): " + string.Join(", ", errors));
                return View(publisher);
            }

            Console.WriteLine($"Dodaję wydawcę: {publisher.Name}");

            _context.Add(publisher);
            await _context.SaveChangesAsync();
            Console.WriteLine("Wydawca zapisany!");

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher != null)
            {
                _context.Publishers.Remove(publisher);
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

            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Publisher publisher)
        {
            if (!ModelState.IsValid)
            {
                return View(publisher);
            }

            try
            {
                _context.Update(publisher);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Publishers.Any(e => e.PublisherId == publisher.PublisherId))
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
