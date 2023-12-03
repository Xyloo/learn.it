import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { UserLoginDto } from '../models/user-login';

@Injectable({
  providedIn: 'root'
})
@Injectable({ providedIn: 'root' })
export class AccountService {
  private userSubject: BehaviorSubject<UserLoginDto | null>;
  public user: Observable<UserLoginDto| null>;

  constructor(
    private router: Router,
    private http: HttpClient
  ) {
    this.userSubject = new BehaviorSubject(JSON.parse(localStorage.getItem('token')!));
    this.user = this.userSubject.asObservable();
  }

  public get userValue() {
    return this.userSubject.value;
  }

  login(username: string, password: string) {
    return this.http.post<UserLoginDto>(`${environment.apiUrl}/users/login`, { username, password })
      .pipe(map(token => {        
        localStorage.setItem('token', JSON.stringify(token));
        this.userSubject.next(token);
        return token;
      }));
  }

  logout() {    
    localStorage.removeItem('token');
    this.userSubject.next(null);
    this.router.navigate(['/login']);
  }
}
