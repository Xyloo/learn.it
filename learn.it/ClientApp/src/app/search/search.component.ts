import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { SearchResultDto } from '../models/searchesult.dto';
import { ActivatedRoute, Router } from '@angular/router';
import { StudySetsService } from '../services/study-sets/study-sets.service';
import { StudySet } from '../models/study-sets/study-set';
import { GroupsService } from '../services/groups/groups.service';
import { forkJoin } from 'rxjs';
import { GroupDto } from '../models/groups/group.dto';
import { SnackbarService } from '../services/snackbar.service';


@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {

  searchResults: SearchResultDto[] = [];
  searchedText: string = '';
  displayLimit: number = 6;
  maxDisplayLimit: number = 15;
  isExpanded: boolean = false;
  buttonText: string = 'Więcej';

  constructor(
    private location: Location,
    private router: Router,
    private route: ActivatedRoute,
    private studySetsService: StudySetsService,
    private groupService: GroupsService,
    private snackBarService: SnackbarService
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const query = params['query'];
      if (query) {
        forkJoin([
          this.studySetsService.findStudySets(query),
          this.groupService.findGroups(query)
        ]).subscribe(results => {
          const [firstResult, secondResult] = results;
          this.searchResults = [...firstResult, ...secondResult];
          console.log("Search results: ", this.searchResults)
        });
      }
    });

  }


  toggleItemsDisplay(): void {
    this.isExpanded = !this.isExpanded;
    if (this.isExpanded) {
      this.displayLimit = this.searchResults.length % 15;
      this.buttonText = 'Schowaj';
    } else {
      this.displayLimit = 6;
      this.buttonText = 'Więcej';
    }
  }

  goBack() {
    this.location.back();
  }

  goToSet(id: number): void {
    this.router.navigate(['/set', id]);
  }

  goToGroup(id: number): void {
    this.groupService.sendJoinRequest(id).subscribe({
      next: () => {
        this.snackBarService.openSnackBar('Wysłano prośbę o dołączenie do grupy.');
      },
      error: (error) => {
        this.snackBarService.openSnackBar(error.error.detail);
      }
    });
  }

  isStudySet(result: SearchResultDto): boolean {
    return (result as StudySet).visibility !== undefined;
  }

}
