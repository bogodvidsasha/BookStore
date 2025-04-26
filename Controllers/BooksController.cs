using BookStore.Models;
using BookStore.Services;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;

        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAll()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> Create(Book book)
        {
            var created = await _bookService.AddBookAsync(book);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Book book)
        {
            if (id != book.Id)
                return BadRequest();

            await _bookService.UpdateBookAsync(book);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        // Додатковий ендпоінт для демонстрації відкату міграції
        [HttpPost("rollback-migration")]
        public IActionResult RollbackLastMigration()
        {
            using (var scope = HttpContext.RequestServices.CreateScope())
            {
                var migrator = scope.ServiceProvider.GetService<IMigrationRunner>();
                // Відкат до першої міграції (видалення колонки Price)
                migrator.MigrateDown(202404250002);
                return Ok("Migration rolled back successfully. Price column has been removed.");
            }
        }

        // Додатковий ендпоінт для відновлення міграції
        [HttpPost("apply-migration")]
        public IActionResult ApplyMigration()
        {
            using (var scope = HttpContext.RequestServices.CreateScope())
            {
                var migrator = scope.ServiceProvider.GetService<IMigrationRunner>();
                // Застосування останньої міграції (додавання колонки Price)
                migrator.MigrateUp(202404250002);
                return Ok("Migration applied successfully. Price column has been added.");
            }
        }
    }
}