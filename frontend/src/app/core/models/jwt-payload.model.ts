export interface JwtPayload {
  exp: number;
  sub: string;
  role: string;
  email?: string;
  name?: string;
}
