import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Book } from '../models/book.model';
import { Observable, catchError, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private apiUrl: string =  "http://localhost:5105/api/Books";

  constructor(private http: HttpClient) { }

  getBooks(): Observable<Book[]> {
    return this.http.get<Book[]>(this.apiUrl).pipe(
      catchError(this.handleError)
    );
  }

  postBook(newBook: Book): Observable<any> {
    return this.http.post<Book[]>(this.apiUrl, newBook).pipe(
      catchError(this.handleError)
    );
  }

  delete(isbnBook: string): Observable<any> {
    return this.http.delete<Book[]>(`${this.apiUrl}/${isbnBook}`).pipe(
      catchError(this.handleError)
    );
  }

  update(updatedBook: Book): Observable<any> {
    return this.http.put<Book[]>(`${this.apiUrl}/${updatedBook.isbn}`, updatedBook).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    console.error('An error occurred:', error);
    return throwError('Something bad happened; please try again later.');
  }
}
