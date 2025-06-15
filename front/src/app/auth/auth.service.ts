import { Injectable, signal } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { firstValueFrom } from "rxjs";

function parseJwt(token: string): any {
  try {
    return JSON.parse(atob(token.split('.')[1]));
  } catch {
    return null;
  }
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  public readonly isLoggedIn = signal<boolean>(false);
  public readonly email = signal<string | null>(null);

  constructor(private http: HttpClient) {
    const token = localStorage.getItem('auth_token');
    this.isLoggedIn.set(!!token);
    if (token) {
      const payload = parseJwt(token);
      this.email.set(payload?.email ?? null);
    }
  }

  async login(username: string, password: string) {
    try {
      const result = await firstValueFrom(
        this.http.post<{ success: boolean, token?: string }>(
          'http://localhost:5062/api/user/login',
          { username, password }
        )
      );
      if (result.token) {
        localStorage.setItem('auth_token', result.token);
        this.isLoggedIn.set(true);
        const payload = parseJwt(result.token);
        this.email.set(payload?.email ?? null);
        return true;
      }
      this.isLoggedIn.set(false);
      this.email.set(null);
      return false;
    } catch {
      this.isLoggedIn.set(false);
      this.email.set(null);
      return false;
    }
  }

  logout() {
    localStorage.removeItem('auth_token');
    this.isLoggedIn.set(false);
    this.email.set(null);
  }
}