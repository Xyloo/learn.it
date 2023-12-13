import { Component, OnInit } from '@angular/core';
import { ActivityDto } from '../models/lastactivity.dto';
import { StudySetsService } from '../services/study-sets/study-sets.service';
import { StudySet } from '../models/study-sets/study-set';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  styleUrls: ['./home.component.css'],
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {

  public studySets: StudySet[];

  constructor(
    private studySetsService: StudySetsService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.studySetsService.getUserRecommendedSets().subscribe((sets) => {
      this.studySets = sets;
    });
  }



  redirectToSet(id: number | null | undefined) {
    console.log("id:" + id)
    if (id !== null && id !== undefined) {
      this.router.navigateByUrl(`/set/${id}`);
    }
  }


  usersLastActivity: ActivityDto[] = [
    {
      id: 1,
      name: "Nazwa zestawu 1",
      avatarUrl: "/assets/plus-icon.svg",
      username: "Userasdasd1",
      isPrivate: true
    },
    {
      id: 2,
      name: "Nazwa zestawu 2",
      avatarUrl: "/assets/plus-icon.svg",
      username: "User2",
      isPrivate: false
    },
    {
      id: 3,
      name: "Nazwa zestawu 2",
      avatarUrl: "/assets/plus-icon.svg",
      username: "User1",
      isPrivate: true
    }
  ]




}
