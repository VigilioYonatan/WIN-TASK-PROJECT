import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../core/services/task.service';
import { CategoryService } from '../../core/services/category.service';
import { EstadoTarea, TaskItem } from '../../core/models/task.model';
import { TaskFormModalComponent } from './task-form-modal/task-form-modal.component';

@Component({
  selector: 'app-tasks-list',
  standalone: true,
  imports: [CommonModule, FormsModule, TaskFormModalComponent],
  templateUrl: './tasks-list.component.html',
  styleUrls: ['./tasks-list.component.css']
})
export class TasksListComponent implements OnInit {
  taskService = inject(TaskService);
  categoryService = inject(CategoryService);

  showModal = signal<boolean>(false);
  taskToEdit = signal<TaskItem | null>(null);
  errorMessage = signal<string | null>(null);

  // Filter models
  selectedCategory = signal<number | null>(null);
  selectedStatus = signal<EstadoTarea | null>(null);

  EstadoTarea = EstadoTarea;

  ngOnInit(): void {
    this.categoryService.loadAll().subscribe();
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.loadAll().subscribe({
      error: err => this.errorMessage.set(err.message)
    });
  }

  onFilterCategory(event: Event): void {
    const val = (event.target as HTMLSelectElement).value;
    const catId = val === '' ? null : Number(val);
    this.taskService.selectedCategoryFilter.set(catId);
  }

  onFilterStatus(event: Event): void {
    const val = (event.target as HTMLSelectElement).value;
    const status = val === '' ? null : Number(val) as EstadoTarea;
    this.taskService.selectedStatusFilter.set(status);
  }

  openCreateModal(): void {
    this.taskToEdit.set(null);
    this.showModal.set(true);
  }

  openEditModal(task: TaskItem): void {
    this.taskToEdit.set(task);
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.taskToEdit.set(null);
  }

  onTaskSaved(): void {
    this.closeModal();
    this.loadTasks();
  }

  onDeleteTask(id: number): void {
    if (confirm('¿Estás seguro de que deseas eliminar esta tarea?')) {
      this.taskService.delete(id).subscribe({
        error: err => alert(err.message || 'Error al eliminar la tarea.')
      });
    }
  }

  getStatusClass(estado: EstadoTarea): string {
    switch (estado) {
      case EstadoTarea.Pendiente:
        return 'badge-pendiente';
      case EstadoTarea.EnProgreso:
        return 'badge-enprogreso';
      case EstadoTarea.Completada:
        return 'badge-completada';
      case EstadoTarea.Cancelada:
        return 'badge-cancelada';
      default:
        return '';
    }
  }
}
