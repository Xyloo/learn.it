import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { GroupDetails } from '../../models/groups/group-details';
import { GroupDto } from '../../models/groups/group.dto';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GroupsService {

  constructor(private http: HttpClient) { }

  leaveGroup(groupId: number) {
    return this.http.get(`${environment.apiUrl}/groups/${groupId}/leave`);
  }

  acceptInvitation(groupId: number) {
    return this.http.get(`${environment.apiUrl}/groups/${groupId}/invite/accept`);
  }

  declineInvitation(groupId: number) {
    return this.http.get(`${environment.apiUrl}/groups/${groupId}/invite/decline`);
  }

  createGroup(name: string) {
    return this.http.post(`${environment.apiUrl}/groups`, name);
  }

  getGroupDetails(groupId: number) {
    return this.http.get<GroupDetails>(`${environment.apiUrl}/groups/${groupId}`);
  }

  updateGroup(groupId: number, groupName: string) {
    return this.http.put(`${environment.apiUrl}/groups/${groupId}`, { name: groupName });
  }

  inviteUser(groupId: number, username: string) {
    return this.http.get(`${environment.apiUrl}/groups/${groupId}/invite/${username}`,);
  }

  findGroups(query: string): Observable<GroupDto[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/groups/find/${query}`).pipe(
      map((response: any[]) => response.map(item => ({
        id: item.groupId,
        name: item.name,
        count: item.userCount,
        creator: {
          username: item.creator.username,
          avatar: item.creator.avatar
        }
      })))
    );
  }

  sendJoinRequest(groupId: number) {
    return this.http.get(`${environment.apiUrl}/groups/${groupId}/join`);
  }


}
