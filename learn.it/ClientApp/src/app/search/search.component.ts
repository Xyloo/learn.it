import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { SearchResultDto } from '../models/searchesult.dto';
import { ActivatedRoute, Router } from '@angular/router';
import { StudySetsService } from '../services/study-sets/study-sets.service';
import { StudySet } from '../models/study-sets/study-set';


@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit{

  searchResults: StudySet[] = [];
  searchedText: string = '';
  displayLimit: number = 6;
  maxDisplayLimit: number = 15;
  isExpanded: boolean = false;
  buttonText: string = 'Więcej';
  constructor(
    private location: Location,
    private router: Router,
    private route: ActivatedRoute,
    private studySetsService: StudySetsService
  ) { }

    ngOnInit(): void {
      this.route.queryParams.subscribe(params => {
        const query = params['query'];
        if (query) {
          this.studySetsService.findStudySets(query).subscribe(data => {
            this.searchResults = data;
            this.searchedText
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

  goToSet(setId: number): void {
    this.router.navigate(['/set', setId]);
  }

}
