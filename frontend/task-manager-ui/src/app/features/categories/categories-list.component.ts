import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CategoryService } from '../../core/services/category.service';
import { Category } from '../../core/models/category.model';

@Component({
  selector: 'app-categories-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './categories-list.component.html',
  styleUrls: ['./categories-list.component.css']
})
export class CategoriesListComponent implements OnInit {
  categoryService = inject(CategoryService);
  private fb = inject(FormBuilder);

  categoryForm: FormGroup = this.fb.group({
    nombre: ['', [Validators.required, Validators.maxLength(80)]]
  });

  editingCategory = signal<Category | null>(null);
  showModal = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.categoryService.loadAll().subscribe({
      error: err => this.errorMessage.set(err.message)
    });
  }

  openCreateModal(): void {
    this.editingCategory.set(null);
    this.categoryForm.reset();
    this.errorMessage.set(null);
    this.showModal.set(true);
  }

  openEditModal(category: Category): void {
    this.editingCategory.set(category);
    this.categoryForm.patchValue({ nombre: category.nombre });
    this.errorMessage.set(null);
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingCategory.set(null);
    this.categoryForm.reset();
  }

  onSubmit(): void {
    if (this.categoryForm.invalid) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    const current = this.editingCategory();
    const nombre = this.categoryForm.value.nombre;

    if (current) {
      this.categoryService.update(current.id, { nombre }).subscribe({
        next: () => this.closeModal(),
        error: err => this.errorMessage.set(err.message)
      });
    } else {
      this.categoryService.create({ nombre }).subscribe({
        next: () => this.closeModal(),
        error: err => this.errorMessage.set(err.message)
      });
    }
  }

  onDelete(id: number): void {
    if (confirm('¿Estás seguro de que deseas eliminar esta categoría?')) {
      this.categoryService.delete(id).subscribe({
        error: err => alert(err.message || 'No se pudo eliminar la categoría.')
      });
    }
  }
}
