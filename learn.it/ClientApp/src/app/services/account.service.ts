import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { UserLoginDto } from '../models/user/user-login';

@Injectable({
  providedIn: 'root'
})
@Injectable({ providedIn: 'root' })
export class AccountService {
  private userSubject: BehaviorSubject<UserLoginDto | null>;
  public user: Observable<UserLoginDto| null>;

  constructor(private router: Router, private http: HttpClient) {
    const storedToken = localStorage.getItem('token');
    const storedUserId = localStorage.getItem('userId');
    let user: UserLoginDto | null = null;
    if (storedToken && storedUserId) {
      user = { token: storedToken, userId: parseInt(storedUserId) };
    }

    this.userSubject = new BehaviorSubject<UserLoginDto | null>(user);
  }

  public get userValue() {
    return this.userSubject.value;
  }

  login(username: string, password: string) {
    return this.http.post<UserLoginDto>(`${environment.apiUrl}/users/login`, { username, password })
      .pipe(map(response => {        
        localStorage.setItem('token', response.token);
        localStorage.setItem('userId', JSON.stringify(response.userId));
        this.userSubject.next(response);
        return response;
      }));
  }

  register(username: string, email: string, password: string) {
    return this.http.post(`${environment.apiUrl}/users/register`, { username, email, password })
  };



  logout() {    
    localStorage.removeItem('token');
    localStorage.removeItem('userId');
    this.userSubject.next(null);
    this.router.navigate(['/login']);
  }
}
