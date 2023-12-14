import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { GroupsService } from '../services/groups/groups.service';
import { GroupDetails } from '../models/groups/group-details';
import { ActivatedRoute, Router } from '@angular/router';
import { SnackbarService } from '../services/snackbar.service';
import { MatDialog } from '@angular/material/dialog';
import { InviteUserDialogComponent } from '../invite-user-dialog/invite-user-dialog.component';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-group-details',
  templateUrl: './group-details.component.html',
  styleUrls: ['./group-details.component.css']
})
export class GroupDetailsComponent implements OnInit {

  groupDetails: GroupDetails = {
    groupId: -1,
    name: '',
    creator: {
      username: '',
      avatar: null
    },
    users: [],
    studySets: []
  };

  isGroupCreator: boolean = false;

  constructor(
    private location: Location,
    private groupService: GroupsService,
    private accountService: AccountService,
    private route: ActivatedRoute,
    private rotuer: Router,
    private snackBarService: SnackbarService,
    private dialog: MatDialog

  ) { }

  ngOnInit(): void {
    const groupId = this.route.snapshot.paramMap.get('id');
    if (groupId) {
      this.groupService.getGroupDetails(Number(groupId))
        .subscribe({
          next: (result) => {
            this.groupDetails = result;
            this.isGroupCreator = result.creator.username === this.accountService.userValue?.uniqueName;
          },
          error: (error) => {
            console.log(error);
          }
        });
    }
  }

  redirectToSet(studySetId: number) {
    this.rotuer.navigate(['/set', studySetId]);
  }


  goBack() {
    this.location.back();
  }


  saveChanges() {

    this.groupService.updateGroup(this.groupDetails.groupId, this.groupDetails.name).subscribe({
        next: () => {
          this.snackBarService.openSnackBar("Pomyślnie zaktualizowano nazwę grupy.", "Zamknij");
        },
      error: (error) => {
        this.snackBarService.openSnackBar(error.error.errors.Name);
        }
      });
  }

  inviteUser() {
    const dialogRef = this.dialog.open(InviteUserDialogComponent, {
      width: '270px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.groupService.inviteUser(this.groupDetails.groupId, result).subscribe({
          next: () => {
            this.snackBarService.openSnackBar(`Zaproszenie zostało pomyślnie wysłane.`, "Zamknij");
          },
          error: () => {
            this.snackBarService.openSnackBar("Wystąpił błąd wysyłania zaproszenia.", "Zamknij");
          }
        });
      }
    });
  }



}
