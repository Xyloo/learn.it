import { Component, OnInit } from '@angular/core';
import { GroupsService } from '../services/groups/groups.service';
import { SnackbarService } from '../services/snackbar.service';
import { UserGroupDto } from '../models/user-groups.dto';
import { UserService } from '../services/user/user.service';
import { Location } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-groups',
  templateUrl: './user-groups.component.html',
  styleUrls: ['./user-groups.component.css']
})
export class UserGroupsComponent implements OnInit{

  userGroups: UserGroupDto[] = [];

  constructor(
    private groupsService: GroupsService,
    private userService: UserService,
    private snackBarService: SnackbarService,
    private location: Location,
    private router: Router
      
  ) { }
    ngOnInit(): void {
      this.userService.getUserGroups().subscribe(groups => {
        this.userGroups = groups;
      });
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

  goBack() {
    this.location.back();
  }

  redirectToGroup(id: number): void {
    this.router.navigate(['/groups', id]);
  }

}
