using System.Transactions;
using System.Xml;

namespace books_backend.Model
{
    public interface IBooksManager
    {
        Task<BookStore> GetBooks();
        Task<Book> GetBookByIsbn(string bookIsbn);
        Task<BookStore> AddBook(Book newBook);
        Task<BookStore> DeleteBook(string bookIsbn);
        Task<BookStore> UpdateBook(string bookIsbn, Book updatedBook);
    }
    public class BooksManager: IBooksManager
    {
        private readonly string xmlPath;
        private readonly BookStore bookStore;
        public BooksManager() {
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Utils");
            xmlPath = Path.Combine(directoryPath, "bookStore.xml");
            bookStore = new BookStore();
            ReadBooks();
        }

        public async Task<BookStore> GetBooks()
        {
            return await Task.FromResult(bookStore);
        }

        public async Task<Book> GetBookByIsbn(string bookIsbn)
        {
            Book b = bookStore.Books.Find(book => book.Isbn == bookIsbn);
            if (b == null)
            {
                throw new ArgumentException($"Book with ISBN {bookIsbn} not found.");
            }
            return await Task.FromResult(b);
        }


        public async Task<BookStore> AddBook(Book newBook)
        {
            bookStore.Books.Add(newBook);
            UpdateXmlFile(newBook);

            return await Task.FromResult(bookStore);
        }

        public async Task<BookStore> DeleteBook(string bookIsbn)
        {
            var bookToRemove = bookStore.Books.Find(book => book.Isbn == bookIsbn);
            if (bookToRemove != null)
            {
                bookStore.Books.Remove(bookToRemove);
                await DeleteFromXmlFile(bookIsbn);
            }
            return bookStore;
        }

        public async Task<BookStore> UpdateBook(string bookIsbn, Book updatedBook)
        {
            var bookToUpdate = bookStore.Books.Find(book => book.Isbn == bookIsbn);
            if (bookToUpdate != null)
            {
                // Update the details of the found book
                bookToUpdate.Title = updatedBook.Title;
                bookToUpdate.Authors = updatedBook.Authors;
                bookToUpdate.Year = updatedBook.Year;
                bookToUpdate.Price = updatedBook.Price;
                bookToUpdate.Category = updatedBook.Category;
                bookToUpdate.Language = updatedBook.Language;
                bookToUpdate.Cover = updatedBook.Cover;
                await UpdateXmlFile();
                return bookStore;
            }
            else
            {
                throw new ArgumentException($"Book with ISBN {bookIsbn} not found.");
            }
        }

        private void ReadBooks()
        {
            if (!File.Exists(xmlPath))
                return;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            XmlNodeList bookNodes = xmlDoc.SelectNodes("//book");
            foreach (XmlNode bookNode in bookNodes)
            {
                Book book = new Book
                {
                    Isbn = bookNode.SelectSingleNode("isbn").InnerText,
                    Title = bookNode.SelectSingleNode("title").InnerText,
                    Authors = new List<string>(),
                    Year = int.Parse(bookNode.SelectSingleNode("year").InnerText),
                    Price = double.Parse(bookNode.SelectSingleNode("price").InnerText),
                    Category = bookNode.Attributes["category"].Value, 
                    Language = bookNode.SelectSingleNode("title").Attributes["lang"].Value
                };

                //Check if there is cover
                XmlAttribute coverAttribute = bookNode.Attributes["cover"];
                if (coverAttribute != null)
                {
                    book.Cover = coverAttribute.Value;
                }

                // Read authors if available
                XmlNodeList authorNodes = bookNode.SelectNodes("author");
                foreach (XmlNode authorNode in authorNodes)
                {
                    book.Authors.Add(authorNode.InnerText);
                }
                bookStore.Books.Add(book);
            }
        }

        private async Task UpdateXmlFile(Book newBook = null)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            if (newBook != null)
            {
                // Add new book node to the XML document
                XmlNode rootNode = xmlDoc.SelectSingleNode("//bookstore");
                XmlNode bookNode = xmlDoc.CreateElement("book");
                XmlAttribute categoryAttribute = xmlDoc.CreateAttribute("category");
                XmlAttribute coverAttribute = xmlDoc.CreateAttribute("cover");
                categoryAttribute.Value = newBook.Category;
                coverAttribute.Value = newBook.Cover;
                bookNode.Attributes.Append(categoryAttribute);
                bookNode.Attributes.Append(coverAttribute);
                XmlNode isbnNode = xmlDoc.CreateElement("isbn");
                isbnNode.InnerText = newBook.Isbn;
                bookNode.AppendChild(isbnNode);
                XmlNode titleNode = xmlDoc.CreateElement("title");
                XmlAttribute langAttribute = xmlDoc.CreateAttribute("lang");
                langAttribute.Value = newBook.Language;
                titleNode.Attributes.Append(langAttribute);
                titleNode.InnerText = newBook.Title;
                bookNode.AppendChild(titleNode);
                foreach (var author in newBook.Authors)
                {
                    XmlNode authorNode = xmlDoc.CreateElement("author");
                    authorNode.InnerText = author;
                    bookNode.AppendChild(authorNode);
                }
                XmlNode yearNode = xmlDoc.CreateElement("year");
                yearNode.InnerText = newBook.Year.ToString();
                bookNode.AppendChild(yearNode);
                XmlNode priceNode = xmlDoc.CreateElement("price"); 
                priceNode.InnerText = newBook.Price.ToString();
                bookNode.AppendChild(priceNode);

                rootNode.AppendChild(bookNode);
            }

            // Save changes to the XML file
            await Task.Run(() => xmlDoc.Save(xmlPath));
        }

        private async Task DeleteFromXmlFile(string bookIsbn)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            XmlNodeList bookNodes = xmlDoc.SelectNodes($"//book[isbn='{bookIsbn}']");
            if (bookNodes?.Count > 0)
            {
                // Remove the book node from the XML document
                foreach (XmlNode node in bookNodes)
                {
                    node.ParentNode?.RemoveChild(node);
                }

                // Save changes to the XML file
                await Task.Run(() => xmlDoc.Save(xmlPath));
            }
        }
    }
}
