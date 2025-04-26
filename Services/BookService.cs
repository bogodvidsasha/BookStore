using BookStore.Models;
using BookStore.Repositories;
using LazyCache;

namespace BookStore.Services
{
    public class BookService
    {
        private readonly IBookRepository _repository;
        private readonly IAppCache _cache;
        private const string AllBooksCacheKey = "all_books";
        private const string BookCacheKeyPrefix = "book_";

        public BookService(IBookRepository repository, IAppCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            // Отримання даних з кешу або з репозиторію, якщо в кеші немає
            return await _cache.GetOrAddAsync(AllBooksCacheKey, async () =>
            {
                Console.WriteLine("Fetching books from database..."); // Логування для демонстрації кешування
                return await _repository.GetAllAsync();
            }, TimeSpan.FromMinutes(5));
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            string cacheKey = $"{BookCacheKeyPrefix}{id}";
            return await _cache.GetOrAddAsync(cacheKey, async () =>
            {
                Console.WriteLine($"Fetching book {id} from database..."); // Логування для демонстрації кешування
                return await _repository.GetByIdAsync(id);
            }, TimeSpan.FromMinutes(5));
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            var result = await _repository.AddAsync(book);
            // Інвалідація кешу після зміни даних
            _cache.Remove(AllBooksCacheKey);
            Console.WriteLine("Cache invalidated after adding a book");
            return result;
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            var result = await _repository.UpdateAsync(book);
            // Інвалідація кешу
            _cache.Remove(AllBooksCacheKey);
            _cache.Remove($"{BookCacheKeyPrefix}{book.Id}");
            Console.WriteLine($"Cache invalidated after updating book {book.Id}");
            return result;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var result = await _repository.DeleteAsync(id);
            // Інвалідація кешу
            _cache.Remove(AllBooksCacheKey);
            _cache.Remove($"{BookCacheKeyPrefix}{id}");
            Console.WriteLine($"Cache invalidated after deleting book {id}");
            return result;
        }
    }
}