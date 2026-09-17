export interface User {
  id: number;
  nombre: string;
  email: string;
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  usuario: User;
  expiresAt: string;
}

export interface RegisterRequest {
  nombre: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}
