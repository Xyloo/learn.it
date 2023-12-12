import { Component } from '@angular/core';
import { UserService } from '../services/user/user.service';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-invite-user-dialog',
  templateUrl: './invite-user-dialog.component.html',
  styleUrls: ['./invite-user-dialog.component.css']
})
export class InviteUserDialogComponent {
  username: string = '';
  userNotFound: boolean = false;
  constructor(
    private userService: UserService,
    private dialogRef: MatDialogRef<InviteUserDialogComponent>
  ) { }

  addUser() {
    this.userService.getUserByUsername(this.username).subscribe(
      user => {
        if (user) {
          this.dialogRef.close(user);
        }
        else {
          this.userNotFound = true;
        }
      },
      error => {
        console.error('Error fetching user:', error);
        this.userNotFound = true;
      }
    );
  }


}
