import { OnInit, Component } from '@angular/core';
import { Book } from '../models/book.model';
import { BookService } from '../services/book.service';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-books',
  templateUrl: './books.component.html',
  styleUrls: ['./books.component.css']
})

export class BooksComponent implements OnInit {

  public displayedColumns: string[] = ['isbn', 'title', 'authors', 'category', 'cover', 'language', 'price', 'year', 'action'];
  public enableEdit: boolean = false;
  public enableEditIndex: number = 0;
  public books: MatTableDataSource<Book> = new MatTableDataSource<Book>();

  constructor(private bookService: BookService) { }

  ngOnInit(): void {
    this.getBooksFromXML();
  }

  formatData(data: any, column: string): string {
    if (column === 'price') {
      return `$${data.toFixed(2)}`; // Format price as currency
    } else if (column === 'year') {
      return data.toString(); // Format year as string
    } else if (column === 'authors') {
      return data.join(', '); // Join authors with comma
    }
    return data; // Return data as is for other columns
  }

  async getBooksFromXML(): Promise<void> {
    try {
      this.bookService.getBooks().subscribe(
        (res: any) => {
          this.books = new MatTableDataSource<Book>(res.books); // Initialize MatTableDataSource
        },
      );
    } catch (error) {
      console.error('Error fetching books:', error);
      this.showFeedback("Error fetching books", false);
    }
  }

  async addBook(book: Book): Promise<void> {
    try {
      await this.bookService.postBook(book).subscribe();
    } catch (error) {
      console.error('Error inserting a new book:', error);
      this.showFeedback("Error inserting a book", false);

    }
  }

  async deleteBook(book: Book): Promise<void> {
    try {
      await this.bookService.delete(book.isbn).subscribe((res: any) => {
        const data: Book[] = this.books.data.filter(b => b !== book);
        this.books.data = data;
        this.showFeedback("Book deleted successfully", true);
      });
    } catch (error) {
      console.error('Error deleting book:', error);
      this.showFeedback("Error deleting book", false);
    }
  }

  async updateBook(book: Book): Promise<void> {
    try {
      await this.bookService.update(book).subscribe((res: any) => {
        this.showFeedback("Book updated successfully", true);
      });
    } catch (error) {
      console.error('Error updating book:', error);
      this.showFeedback("Error updating book", false);
    }
  }

  showFeedback(message: string, success: boolean): void {
    Swal.fire({
      icon: success ? "success": "error",
      title: success ? "Good Job!": "Something went wrong!",
      text: message,
      showConfirmButton: false,
      timer: 1500
    });
  }


  onEdit(rowIndex: number): void {
    this.enableEdit = true;
    this.enableEditIndex = rowIndex;
    console.log(this.enableEditIndex);
  }

  onDelete(book: Book): void {
    this.enableEdit = false;
    this.deleteBook(book);
  }

  onSave(book: Book): void {
    this.enableEdit = false;
    if (typeof book.authors === 'string') {
      const authorsString: any = book.authors; // Assuming book.authors is of type any or unknown
      const authorsList: string[] = authorsString.split(','); // Split the string by comma to get an array
      book.authors = authorsList;    // Update the book with the new authors array
    }
    this.updateBook(book);
  }

  onCancel(): void {
    this.enableEdit = false;
  }

  onAdd(): void {
    let uniqueIsbn = this.generateUniqueIsbn();
    let currentYear = new Date().getFullYear();
    const newBook: Book = {
      isbn: uniqueIsbn,
      title: '',
      authors: [],
      year: currentYear,
      price: 30,
      category: '',
      language: '',
      cover: ''
    };

    this.books.data.push(newBook);
    this.books._updateChangeSubscription();
    this.enableEdit = true;    // Enable editing mode for the new book
    this.enableEditIndex = this.books.data.length - 1;    // Set enableEditIndex to the index of the newly added book
    this.addBook(newBook);
    console.log(this.enableEditIndex);
  }

  generateUniqueIsbn(): string {
    let randomIsbn: string;
    let isUnique: boolean = false;
    do {    // Loop until a unique ISBN is generated
      // Generate a random 13-digit number as a string
      randomIsbn = Math.floor(1000000000000 + Math.random() * 9000000000000).toString();
      // Check if the generated ISBN already exists in the list of books
      isUnique = !this.books.data.find(book => book.isbn === randomIsbn);
    } while (!isUnique);
    return randomIsbn;
  }
}

