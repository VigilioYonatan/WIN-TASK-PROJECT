export interface Category {
  id: number;
  nombre: string;
  createdAt: string;
  totalTasks?: number;
}

export interface CreateCategoryRequest {
  nombre: string;
}

export interface UpdateCategoryRequest {
  nombre: string;
}
