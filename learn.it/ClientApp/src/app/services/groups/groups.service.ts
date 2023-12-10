import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

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

}
