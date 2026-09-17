import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Category, CreateCategoryRequest, UpdateCategoryRequest } from '../models/category.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private readonly apiUrl = 'http://localhost:5000/api/categories';

  categories = signal<Category[]>([]);
  isLoading = signal<boolean>(false);

  constructor(private http: HttpClient) {}

  loadAll(): Observable<Category[]> {
    this.isLoading.set(true);
    return this.http.get<Category[]>(this.apiUrl).pipe(
      tap({
        next: cats => {
          this.categories.set(cats);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      })
    );
  }

  getById(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateCategoryRequest): Observable<Category> {
    return this.http.post<Category>(this.apiUrl, request).pipe(
      tap(newCat => {
        this.categories.update(current => [...current, newCat]);
      })
    );
  }

  update(id: number, request: UpdateCategoryRequest): Observable<Category> {
    return this.http.put<Category>(`${this.apiUrl}/${id}`, request).pipe(
      tap(updatedCat => {
        this.categories.update(current =>
          current.map(c => (c.id === id ? updatedCat : c))
        );
      })
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => {
        this.categories.update(current => current.filter(c => c.id !== id));
      })
    );
  }
}
