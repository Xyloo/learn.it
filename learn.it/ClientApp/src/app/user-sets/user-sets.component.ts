import { Component, OnInit } from '@angular/core';
import { UserSetDto } from '../models/user-sets.dto';
import { Location } from '@angular/common';
import { StudySetsService } from '../services/study-sets/study-sets.service';
import { style } from '@angular/animations';

@Component({
  selector: 'app-user-sets',
  templateUrl: './user-sets.component.html',
  styleUrls: ['./user-sets.component.css']
})
export class UserSetsComponent implements OnInit {

  userSets: UserSetDto[] = [];
  constructor(private location: Location, private studySetService: StudySetsService) { }
  ngOnInit(): void {
    this.studySetService.getStudySets().subscribe({
      next: (sets) => {
        this.userSets = sets; 
      },
      error: (error) => {
        console.error('Error fetching study sets:', error);
      }
    });  
  }

  //usuwanie info gdy nie jestes group ownerem 
  deleteSet(itemId: number) {
    this.studySetService.deleteStudySet(itemId).subscribe({
      next: () => {
        console.log('Study set deleted successfully');
      },
      error: (error) => {
        console.error('Error deleting study set:', error);
      }
    });
  }

  goBack() {
    this.location.back(); 
  }

}
