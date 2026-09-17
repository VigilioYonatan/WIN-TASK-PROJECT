export enum EstadoTarea {
  Pendiente = 0,
  EnProgreso = 1,
  Completada = 2,
  Cancelada = 3
}

export interface TaskItem {
  id: number;
  titulo: string;
  descripcion: string;
  fechaInicio: string;
  fechaCierre: string;
  estado: EstadoTarea;
  estadoDescripcion: string;
  categoriaId: number;
  categoriaNombre?: string;
  usuarioId: number;
  usuarioNombre?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateTaskRequest {
  titulo: string;
  descripcion: string;
  fechaInicio: string;
  fechaCierre: string;
  categoriaId: number;
  estado?: EstadoTarea;
}

export interface UpdateTaskRequest {
  titulo: string;
  descripcion: string;
  fechaInicio: string;
  fechaCierre: string;
  categoriaId: number;
  estado: EstadoTarea;
}
