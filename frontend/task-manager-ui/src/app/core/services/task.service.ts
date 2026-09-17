import { Injectable, signal, computed } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { CreateTaskRequest, EstadoTarea, TaskItem, UpdateTaskRequest } from '../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private readonly apiUrl = 'http://localhost:5000/api/tasks';

  // Signals State
  tasks = signal<TaskItem[]>([]);
  isLoading = signal<boolean>(false);
  selectedCategoryFilter = signal<number | null>(null);
  selectedStatusFilter = signal<EstadoTarea | null>(null);

  // Computed signals
  filteredTasks = computed(() => {
    let result = this.tasks();
    const catId = this.selectedCategoryFilter();
    const status = this.selectedStatusFilter();

    if (catId !== null) {
      result = result.filter(t => t.categoriaId === catId);
    }
    if (status !== null) {
      result = result.filter(t => t.estado === status);
    }
    return result;
  });

  taskStats = computed(() => {
    const all = this.tasks();
    return {
      total: all.length,
      pendientes: all.filter(t => t.estado === EstadoTarea.Pendiente).length,
      enProgreso: all.filter(t => t.estado === EstadoTarea.EnProgreso).length,
      completadas: all.filter(t => t.estado === EstadoTarea.Completada).length,
    };
  });

  constructor(private http: HttpClient) {}

  loadAll(categoriaId?: number, estado?: EstadoTarea): Observable<TaskItem[]> {
    this.isLoading.set(true);
    let params = new HttpParams();
    if (categoriaId !== undefined && categoriaId !== null) {
      params = params.set('categoriaId', categoriaId.toString());
    }
    if (estado !== undefined && estado !== null) {
      params = params.set('estado', estado.toString());
    }

    return this.http.get<TaskItem[]>(this.apiUrl, { params }).pipe(
      tap({
        next: items => {
          this.tasks.set(items);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      })
    );
  }

  getById(id: number): Observable<TaskItem> {
    return this.http.get<TaskItem>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateTaskRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.apiUrl, request).pipe(
      tap(created => {
        this.tasks.update(current => [created, ...current]);
      })
    );
  }

  update(id: number, request: UpdateTaskRequest): Observable<TaskItem> {
    return this.http.put<TaskItem>(`${this.apiUrl}/${id}`, request).pipe(
      tap(updated => {
        this.tasks.update(current =>
          current.map(t => (t.id === id ? updated : t))
        );
      })
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => {
        this.tasks.update(current => current.filter(t => t.id !== id));
      })
    );
  }
}
