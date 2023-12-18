import { Component, OnInit } from '@angular/core';

import { StudySetsService } from '../services/study-sets/study-sets.service';
import { StudySet } from '../models/study-sets/study-set';
import { Router } from '@angular/router';
import { LastActivityDto } from '../models/user/last-activity.dto';
import { UserService } from '../services/user/user.service';
import { AchievementsDto } from '../models/achievements/user-achievements.dto';
import { SnackbarService } from '../services/snackbar.service';

@Component({
  selector: 'app-home',
  styleUrls: ['./home.component.css'],
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {

  public studySets: StudySet[] = [];
  public userLastActivity: LastActivityDto[] = []
  public userAchievements: AchievementsDto[] = []

  constructor(
    private studySetsService: StudySetsService,
    private userService: UserService,
    private snackBarService: SnackbarService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.studySetsService.getUserRecommendedSets().subscribe({
      next: (sets) => {
        this.studySets = sets; },
      error: () => {
        this.snackBarService.openSnackBar('Błąd serwera podczas ładowania rekomendowanych zestawów.');}
    });
    this.userService.getLastActivity().subscribe({
      next: (activities) => {
        this.userLastActivity = activities;
      },
      error: () => {
        this.snackBarService.openSnackBar('Błąd serwera podczas ładowania ostatniej aktywności.');}
    });
    this.userService.getAchievements().subscribe({
      next: (achievements) => {
        this.userAchievements = achievements;
      },
      error: () => {
        this.snackBarService.openSnackBar('Błąd serwera podczas pobierania osiągnieć.'); }
    });
  }


  redirectToSet(id: number | null | undefined) {
    console.log("id:" + id)
    if (id !== null && id !== undefined) {
      this.router.navigateByUrl(`/set/${id}`);
    }
  }
}
