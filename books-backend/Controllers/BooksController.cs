using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using books_backend.Model;
using System.Transactions;

namespace books_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBooksManager booksService;

        public BooksController(IBooksManager _booksService)
        {
            booksService = _booksService;
        }

        //Get the Book Store 
        [HttpGet]
        public async Task<ActionResult<BookStore>> GetBookStore()
        {
            try
            {
                BookStore bookStore = await booksService.GetBooks();
                return Ok(bookStore);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
             }
        }

        //Get Book by bookId
        [HttpGet("/{bookIsbn}")]
        public async Task<ActionResult<Book>> GetBook(string bookIsbn)
        {
            try
            {
                return Ok(await booksService.GetBookByIsbn(bookIsbn));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex);
            }
        }
        //Insert new book to the book store
        [HttpPost]
        public async Task<ActionResult<BookStore>> Post([FromBody] Book newBook)
        {
            try
            {
                var bookStore = await booksService.AddBook(newBook);
                return Ok(bookStore);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Occurred: {ex.Message}");
            }
        }

        //Delete a book from the book store, by book isbn
        [HttpDelete("{bookIsbn}")]
        public async Task<ActionResult<BookStore>> Delete(string bookIsbn)
        {
            try
            {
                var bookStore = await booksService.DeleteBook(bookIsbn);
                return Ok(bookStore);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Occurred: {ex.Message}");
            }
        }

        //Delete a book from the book store, by book isbn
        [HttpPut("{bookIsbn}")]
        public async Task<ActionResult<BookStore>> Put(string bookIsbn, [FromBody] Book updatedBook)
        {
            try
            {
                var bookStore = await booksService.UpdateBook(bookIsbn, updatedBook);
                return Ok(bookStore);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Occurred: {ex.Message}");
            }
        }
    }
}
