export enum UserRole {
  Admin = 0,
  Teacher = 1,
  Student = 2,
}

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  password?: string;
  role: UserRole;
  avatar?: string;
  authProvider: string;
  googleId?: string;
  facebookId?: string;
  createdAt?: Date;
  updatedAt?: Date;
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  password?: string;
  role: UserRole;
  avatar?: string;
  authProvider?: string;
  googleId?: string;
  facebookId?: string;
}

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  password?: string;
  role: UserRole;
  avatar?: string;
  authProvider?: string;
  googleId?: string;
  facebookId?: string;
}

export interface EmailCheckResponse {
  exists: boolean;
}
