import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { UserGroupsInfo } from '../models/groups/user-group-info.dto';
import { UserService } from '../services/user/user.service';
import { AccountService } from '../services/account.service';
import { StudySetsService } from '../services/study-sets/study-sets.service';
import { Router } from '@angular/router';
import { Visibility } from '../models/study-sets/study-set-visibility.dto';

@Component({
  selector: 'app-choose-group-dialog',
  templateUrl: './choose-group-dialog.component.html',
  styleUrls: ['./choose-group-dialog.component.css']
})
export class ChooseGroupDialogComponent implements OnInit {
  Visibility = Visibility;
  groupId: string = '';
  visibility: boolean = false;
  groups: UserGroupsInfo[] = [];
  selectedGroupId: number | null = null;
  visibilityOptions = [
    { label: 'Publiczny', value: Visibility.Publiczny },
    { label: 'Prywatny', value: Visibility.Prywatny },
    { label: 'Grupa', value: Visibility.Grupa }
  ];
  selectedVisibility: Visibility = Visibility.Publiczny;
  setId: number; 

  constructor(
    private userService: UserService,
    private studySetsService: StudySetsService,
    private router: Router,
    private dialogRef: MatDialogRef<ChooseGroupDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.setId = data.setId; 
  }

  ngOnInit() {
    this.userService.getUserGroups().subscribe(groups => {
      this.groups = groups;
    }, error => {
     
    });
  }

  deleteSet() {
    if (this.setId) {
      this.studySetsService.deleteStudySet(this.setId).subscribe({
        next: () => {},
        error: (err) => {},
        complete: () => {
          this.dialogRef.close();
          this.router.navigate(['/']);
        }
      });
    }
  }
}
