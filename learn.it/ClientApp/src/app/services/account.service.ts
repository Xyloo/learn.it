import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { UserLoginDto } from '../models/user/user-login';
import { jwtDecode } from 'jwt-decode';

@Injectable({ providedIn: 'root' })
export class AccountService {
  private userSubject: BehaviorSubject<UserLoginDto | null>;
  public user: Observable<UserLoginDto | null>;

  constructor(private router: Router, private http: HttpClient) {
    const storedToken = localStorage.getItem('token');
    let user: UserLoginDto | null = null;
    if (storedToken) {
      user = this.decodeToken(storedToken);
    }

    this.userSubject = new BehaviorSubject<UserLoginDto | null>(user);
    this.user = this.userSubject.asObservable();
  }

  public get userValue(): UserLoginDto | null {
    return this.userSubject.value;
  }

  login(username: string, password: string) {
    return this.http.post<UserLoginDto>(`${environment.apiUrl}/users/login`, { username, password })
      .pipe(map(response => {
        localStorage.setItem('token', response.token);
        const user = this.decodeToken(response.token);
        this.userSubject.next(user);
        return response;
      }));
  }

  register(username: string, email: string, password: string) {
    return this.http.post(`${environment.apiUrl}/users/register`, { username, email, password });
  }

  logout() {
    localStorage.removeItem('token');
    this.userSubject.next(null);
    this.router.navigate(['/login']);
  }

  private decodeToken(token: string): UserLoginDto | null {
    try {
      const decoded = jwtDecode<any>(token); 
      return {
        token: token,
        role: decoded.role,
        uniqueName: decoded.unique_name,
        email: decoded.email,
        userId: parseInt(decoded.nameid)
      };
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }
}
