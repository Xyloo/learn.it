import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { UserGroupsInfo } from '../../models/groups/user-group-info.dto';
import { Observable, map, of } from 'rxjs';
import { AccountService } from '../account.service';
import { GroupInvitation } from '../../models/groups/group-invitation';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) { }

  getUserGroups(): Observable<UserGroupsInfo[]> {
    const userId = this.accountService.userValue?.userId;
    if (userId === null || userId === undefined) {
      return of([]);
    }

    return this.http.get<any[]>(`${environment.apiUrl}/users/${userId}/groups`).pipe(
      map(groups => groups.map(group => ({
        id: group.groupId,
        name: group.name
      })))
    );
  };

  getUserInvitations(): Observable<GroupInvitation[]> {
    return this.http.get<GroupInvitation[]>(`${environment.apiUrl}/users/${this.accountService.userValue?.userId}/join-requests`);
  }

  getUserByUsername(username: string) {
    return this.http.get<any>(`${environment.apiUrl}/users/${username}`).pipe(
      map(response => response.userId)
    );
  }

  validatePasswordChange(password: string) {
    return this.http.post(`${environment.apiUrl}/users/validate-current-password`, { password });
  }

  changeUserPassword(newPassword: string) {
    return this.http.put(`${environment.apiUrl}/users/${this.accountService.userValue?.userId}`, { password: newPassword });
  }

  uploadAvatar(formData: FormData): Observable<string> {
    return this.http.post(`${environment.apiUrl}/users/avatar/${this.accountService.userValue?.userId}`, formData, { responseType: 'text' });
  }

  removeAvatar(): Observable<string> {
    return this.http.delete(`${environment.apiUrl}/users/avatar/${this.accountService.userValue?.userId}`, { responseType: 'text' });
  }  

  getUserAvatar(userId: number): Observable<string | null> {
    return this.http.get<any>(`${environment.apiUrl}/users/${this.accountService.userValue?.userId}`).pipe(
      map(user => user.avatar)
    );
  }
}
