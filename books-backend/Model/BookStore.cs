using System.Transactions;
using System.Xml.Linq;

namespace books_backend.Model
{
    public class BookStore
    {
        public List<Book> Books { get; set; }

        // Constructor to initialize the list of books
        public BookStore()
        {
            Books = new List<Book>();
        }
    }
}
