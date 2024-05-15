namespace books_backend.Model
{
    public class Book
    {
        public string Isbn { get; set; }
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public int Year { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
        public string Language { get; set; }
        public string Cover { get; set; }

        public Book()
        {
            Authors = new List<string>();
        }

        public Book(string isbn, string title, List<string> authors, int year, double price, string category, string language, string cover)
        {
            Isbn = isbn;
            Title = title;
            Authors = authors;
            Year = year;
            Price = price;
            Category = category;
            Language = language;
            Cover = cover;
        }
    }
}
