export interface Profile {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  bio?: string;
  avatar?: string | null;
  authProvider: string;
  createdAt: Date;
}
