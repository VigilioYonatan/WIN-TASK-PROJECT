import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TaskService } from './task.service';
import { CreateTaskRequest, EstadoTarea, TaskItem } from '../models/task.model';

describe('TaskService', () => {
  let service: TaskService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TaskService]
    });
    service = TestBed.inject(TaskService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should compute task statistics accurately', () => {
    const mockTasks: TaskItem[] = [
      {
        id: 1,
        titulo: 'Tarea 1',
        descripcion: 'Desc 1',
        fechaInicio: new Date().toISOString(),
        fechaCierre: new Date().toISOString(),
        estado: EstadoTarea.Pendiente,
        estadoDescripcion: 'Pendiente',
        categoriaId: 1,
        usuarioId: 1,
        createdAt: new Date().toISOString()
      },
      {
        id: 2,
        titulo: 'Tarea 2',
        descripcion: 'Desc 2',
        fechaInicio: new Date().toISOString(),
        fechaCierre: new Date().toISOString(),
        estado: EstadoTarea.EnProgreso,
        estadoDescripcion: 'EnProgreso',
        categoriaId: 1,
        usuarioId: 1,
        createdAt: new Date().toISOString()
      },
      {
        id: 3,
        titulo: 'Tarea 3',
        descripcion: 'Desc 3',
        fechaInicio: new Date().toISOString(),
        fechaCierre: new Date().toISOString(),
        estado: EstadoTarea.Completada,
        estadoDescripcion: 'Completada',
        categoriaId: 2,
        usuarioId: 1,
        createdAt: new Date().toISOString()
      }
    ];

    service.tasks.set(mockTasks);

    const stats = service.taskStats();
    expect(stats.total).toBe(3);
    expect(stats.pendientes).toBe(1);
    expect(stats.enProgreso).toBe(1);
    expect(stats.completadas).toBe(1);
  });

  it('should send POST request to create task and update signal', () => {
    const newReq: CreateTaskRequest = {
      titulo: 'Nueva Tarea',
      descripcion: 'Detalle',
      fechaInicio: new Date().toISOString(),
      fechaCierre: new Date().toISOString(),
      categoriaId: 1,
      estado: EstadoTarea.Pendiente
    };

    const createdItem: TaskItem = {
      ...newReq,
      id: 10,
      estado: EstadoTarea.Pendiente,
      estadoDescripcion: 'Pendiente',
      usuarioId: 1,
      createdAt: new Date().toISOString()
    };

    service.create(newReq).subscribe(res => {
      expect(res.id).toBe(10);
      expect(service.tasks().length).toBe(1);
      expect(service.tasks()[0].titulo).toBe('Nueva Tarea');
    });

    const req = httpMock.expectOne('http://localhost:5000/api/tasks');
    expect(req.request.method).toBe('POST');
    req.flush(createdItem);
  });
});
