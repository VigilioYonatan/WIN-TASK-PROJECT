import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TaskService } from '../../../core/services/task.service';
import { CategoryService } from '../../../core/services/category.service';
import { EstadoTarea, TaskItem } from '../../../core/models/task.model';

@Component({
  selector: 'app-task-form-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './task-form-modal.component.html',
  styleUrls: ['./task-form-modal.component.css']
})
export class TaskFormModalComponent implements OnInit {
  private fb = inject(FormBuilder);
  private taskService = inject(TaskService);
  categoryService = inject(CategoryService);

  @Input() taskToEdit: TaskItem | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  taskForm!: FormGroup;
  errorMessage = signal<string | null>(null);
  isLoading = signal<boolean>(false);

  estados = [
    { value: EstadoTarea.Pendiente, label: 'Pendiente' },
    { value: EstadoTarea.EnProgreso, label: 'En Progreso' },
    { value: EstadoTarea.Completada, label: 'Completada' },
    { value: EstadoTarea.Cancelada, label: 'Cancelada' }
  ];

  ngOnInit(): void {
    this.categoryService.loadAll().subscribe();
    this.initForm();
  }

  private initForm(): void {
    const defaultStart = new Date().toISOString().substring(0, 16);
    const defaultEnd = new Date(Date.now() + 86400000 * 2).toISOString().substring(0, 16);

    this.taskForm = this.fb.group({
      titulo: [this.taskToEdit?.titulo ?? '', [Validators.required, Validators.maxLength(150)]],
      descripcion: [this.taskToEdit?.descripcion ?? '', [Validators.maxLength(1000)]],
      fechaInicio: [this.taskToEdit ? this.formatDateForInput(this.taskToEdit.fechaInicio) : defaultStart, [Validators.required]],
      fechaCierre: [this.taskToEdit ? this.formatDateForInput(this.taskToEdit.fechaCierre) : defaultEnd, [Validators.required]],
      categoriaId: [this.taskToEdit?.categoriaId ?? (this.categoryService.categories()[0]?.id ?? 1), [Validators.required]],
      estado: [this.taskToEdit?.estado ?? EstadoTarea.Pendiente, [Validators.required]]
    });
  }

  private formatDateForInput(dateStr: string): string {
    const d = new Date(dateStr);
    return new Date(d.getTime() - d.getTimezoneOffset() * 60000).toISOString().substring(0, 16);
  }

  onSubmit(): void {
    if (this.taskForm.invalid) {
      this.taskForm.markAllAsTouched();
      return;
    }

    const formVal = this.taskForm.value;
    if (new Date(formVal.fechaCierre) < new Date(formVal.fechaInicio)) {
      this.errorMessage.set('La fecha de cierre no puede ser anterior a la fecha de inicio.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    const payload = {
      titulo: formVal.titulo,
      descripcion: formVal.descripcion,
      fechaInicio: new Date(formVal.fechaInicio).toISOString(),
      fechaCierre: new Date(formVal.fechaCierre).toISOString(),
      categoriaId: Number(formVal.categoriaId),
      estado: Number(formVal.estado)
    };

    if (this.taskToEdit) {
      this.taskService.update(this.taskToEdit.id, payload).subscribe({
        next: () => {
          this.isLoading.set(false);
          this.saved.emit();
        },
        error: err => {
          this.isLoading.set(false);
          this.errorMessage.set(err.message);
        }
      });
    } else {
      this.taskService.create(payload).subscribe({
        next: () => {
          this.isLoading.set(false);
          this.saved.emit();
        },
        error: err => {
          this.isLoading.set(false);
          this.errorMessage.set(err.message);
        }
      });
    }
  }

  onCancel(): void {
    this.close.emit();
  }
}
