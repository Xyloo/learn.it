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
}
