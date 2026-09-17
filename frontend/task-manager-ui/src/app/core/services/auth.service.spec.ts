import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/user.model';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
    expect(service.isAuthenticated()).toBeFalse();
  });

  it('should authenticate user and store token on successful login', () => {
    const mockRequest: LoginRequest = { email: 'admin@rimac.com', password: 'Password123!' };
    const mockResponse: AuthResponse = {
      token: 'mock-jwt-token-xyz',
      usuario: { id: 1, nombre: 'Admin User', email: 'admin@rimac.com', createdAt: new Date().toISOString() },
      expiresAt: new Date().toISOString()
    };

    service.login(mockRequest).subscribe(res => {
      expect(res.token).toEqual('mock-jwt-token-xyz');
      expect(service.isAuthenticated()).toBeTrue();
      expect(service.currentUser()?.nombre).toEqual('Admin User');
      expect(localStorage.getItem('taskmanager_auth_token')).toEqual('mock-jwt-token-xyz');
    });

    const req = httpMock.expectOne('http://localhost:5000/api/auth/login');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should clear token and user state on logout', () => {
    localStorage.setItem('taskmanager_auth_token', 'temp-token');
    service.currentUser.set({ id: 1, nombre: 'User', email: 'user@rimac.com', createdAt: '' });

    service.logout();

    expect(service.isAuthenticated()).toBeFalse();
    expect(service.currentUser()).toBeNull();
    expect(localStorage.getItem('taskmanager_auth_token')).toBeNull();
  });
});
