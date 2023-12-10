import { Component, OnInit } from '@angular/core';
import { UserSetDto } from '../models/user-sets.dto';
import { Location } from '@angular/common';
import { StudySetsService } from '../services/study-sets/study-sets.service';
import { style } from '@angular/animations';
import { User } from 'oidc-client';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-user-sets',
  templateUrl: './user-sets.component.html',
  styleUrls: ['./user-sets.component.css']
})
export class UserSetsComponent implements OnInit {

  userSets: UserSetDto[] = [];
  currentUsername: string | null = null;

  constructor(
    private location: Location,
    private studySetService: StudySetsService,
    private accountService: AccountService,
    private snackBar: MatSnackBar
  ) { }


  ngOnInit(): void {
    this.currentUsername = this.accountService.userValue ? this.accountService.userValue.uniqueName : null;

    this.studySetService.getStudySets().subscribe({
      next: (sets) => {
        this.userSets = sets;
        console.log(this.userSets); 
      },
      error: (error) => {
        console.error('Error fetching study sets:', error);
      }
    });
  }

  openSnackBar(message: string, action: string = 'Close') {
    this.snackBar.open(message, action, {
      duration: 2000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
    });
  }

  deleteSet(itemId: number) {
    console.log(itemId);
    console.log(this.userSets )
    this.studySetService.deleteStudySet(itemId).subscribe({
      next: () => {
        this.userSets = this.userSets.filter(item => item.id !== itemId);
      },
      error: (error) => {
        this.openSnackBar('Wystąpił błąd podczas usuwania zestawu.', 'Close') 
      }
    });
  }

  canDeleteSet(username: string): boolean {
    return this.currentUsername === username;
  }

  goBack() {
    this.location.back();
  }



}
