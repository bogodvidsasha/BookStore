using BookStore.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BookStore.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly string _connectionString;

        public BookRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            var books = new List<Book>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT * FROM Books", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            books.Add(new Book
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Author = reader.GetString(reader.GetOrdinal("Author")),
                                PublishedYear = reader.IsDBNull(reader.GetOrdinal("PublishedYear")) ? null : reader.GetInt32(reader.GetOrdinal("PublishedYear")),
                                ISBN = reader.IsDBNull(reader.GetOrdinal("ISBN")) ? null : reader.GetString(reader.GetOrdinal("ISBN")),
                                Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? null : reader.GetDecimal(reader.GetOrdinal("Price"))
                            });
                        }
                    }
                }
            }
            return books;
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SELECT * FROM Books WHERE Id = @Id", connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Book
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Author = reader.GetString(reader.GetOrdinal("Author")),
                                PublishedYear = reader.IsDBNull(reader.GetOrdinal("PublishedYear")) ? null : reader.GetInt32(reader.GetOrdinal("PublishedYear")),
                                ISBN = reader.IsDBNull(reader.GetOrdinal("ISBN")) ? null : reader.GetString(reader.GetOrdinal("ISBN")),
                                Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? null : reader.GetDecimal(reader.GetOrdinal("Price"))
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public async Task<Book> AddAsync(Book book)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(
                    "INSERT INTO Books (Title, Author, PublishedYear, ISBN, Price) VALUES (@Title, @Author, @PublishedYear, @ISBN, @Price); SELECT SCOPE_IDENTITY()",
                    connection))
                {
                    command.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = book.Title;
                    command.Parameters.Add("@Author", SqlDbType.NVarChar, 100).Value = book.Author;
                    command.Parameters.Add("@PublishedYear", SqlDbType.Int).Value = book.PublishedYear.HasValue ? (object)book.PublishedYear.Value : DBNull.Value;
                    command.Parameters.Add("@ISBN", SqlDbType.NVarChar, 20).Value = book.ISBN != null ? (object)book.ISBN : DBNull.Value;
                    command.Parameters.Add("@Price", SqlDbType.Decimal).Value = book.Price.HasValue ? (object)book.Price.Value : DBNull.Value;

                    var newId = Convert.ToInt32(await command.ExecuteScalarAsync());
                    book.Id = newId;
                    return book;
                }
            }
        }

        public async Task<Book> UpdateAsync(Book book)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(
                    "UPDATE Books SET Title = @Title, Author = @Author, PublishedYear = @PublishedYear, ISBN = @ISBN, Price = @Price WHERE Id = @Id",
                    connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = book.Id;
                    command.Parameters.Add("@Title", SqlDbType.NVarChar, 100).Value = book.Title;
                    command.Parameters.Add("@Author", SqlDbType.NVarChar, 100).Value = book.Author;
                    command.Parameters.Add("@PublishedYear", SqlDbType.Int).Value = book.PublishedYear.HasValue ? (object)book.PublishedYear.Value : DBNull.Value;
                    command.Parameters.Add("@ISBN", SqlDbType.NVarChar, 20).Value = book.ISBN != null ? (object)book.ISBN : DBNull.Value;
                    command.Parameters.Add("@Price", SqlDbType.Decimal).Value = book.Price.HasValue ? (object)book.Price.Value : DBNull.Value;

                    await command.ExecuteNonQueryAsync();
                    return book;
                }
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("DELETE FROM Books WHERE Id = @Id", connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}