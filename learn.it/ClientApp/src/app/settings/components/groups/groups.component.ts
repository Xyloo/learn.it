import { Component, OnInit } from '@angular/core';
import { UserGroupDto } from '../../../models/user-groups.dto';
import { UserService } from '../../../services/user/user.service';
import { GroupsService } from '../../../services/groups/groups.service';
import { SnackbarService } from '../../../services/snackbar.service';
import { GroupInvitation } from '../../../models/groups/group-invitation';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.css']
})
export class GroupsComponent implements OnInit {

  userGroups: UserGroupDto[] = [];
  userInvitations: GroupInvitation[] = [];
  constructor(
    private userService: UserService,
    private groupsService: GroupsService,
    private snackBarService: SnackbarService
  ) { }

  ngOnInit(): void {
    this.userService.getUserGroups().subscribe(groups => {
      this.userGroups = groups;
    });

    this.userService.getUserInvitations().subscribe(invitations => {
      this.userInvitations = invitations;
    });
    console.log(this.userInvitations)
  }

  leaveGroup(id: number) {
    this.groupsService.leaveGroup(id).subscribe({
      next: () => {
        this.snackBarService.openSnackBar("Pomyślnie opuściłeś grupę.", "Zamknij");
        this.userGroups = this.userGroups.filter(group => group.id !== id);
      },
      error: () => {
        this.snackBarService.openSnackBar("Wystąpił błąd podczas opuszczania grupy.", "Zamknij");
      }
    });
  }

  acceptInvitation(groupId: number) {
    this.groupsService.acceptInvitation(groupId).subscribe({
      next: () => {
        this.snackBarService.openSnackBar("Pomyślnie dołączyłeś do grupy.", "Zamknij");
        this.userInvitations = this.userInvitations.filter(invitation => invitation.groupId !== groupId);
      },
      error: () => {
        this.snackBarService.openSnackBar("Wystąpił błąd podczas dołączania do grupy.", "Zamknij");
      }
    });
  }

  declineInvitation(groupId: number) {
    this.groupsService.declineInvitation(groupId).subscribe({
      next: () => {
        this.snackBarService.openSnackBar("Pomyślnie odrzuciłeś zaproszenie.", "Zamknij");
        this.userInvitations = this.userInvitations.filter(invitation => invitation.groupId !== groupId);
      },
      error: () => {
        this.snackBarService.openSnackBar("Wystąpił błąd podczas odrzucania zaproszenia.", "Zamknij");
      }
    });
  }

}
