import { Component } from '@angular/core';
import { Location } from '@angular/common';
import { SearchResultDto } from '../models/searchesult.dto';
import { Router } from '@angular/router';


@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent {
  constructor(private location: Location, private router: Router) { }

  searchedText: string = '';
  displayLimit: number = 6;
  maxDisplayLimit: number = 15;
  isExpanded: boolean = false;
  buttonText: string = 'Więcej';

   searchResults: SearchResultDto[] = [
    {
       id: 1,
       setName: "Fizyka - wzory",
       setCount: 25,
       avatarUrl: '/assets/temp-avatar.png',
       username: "User1" 
     },
     {
       id: 1,
       setName: "Fizyka - zdjecia",
       setCount: 5,
       avatarUrl: '/assets/temp-avatar.png',
       username: "User2"
     },
     {
       id: 1,
       setName: "Fizyka - wzoryFizyka - wzory",
       setCount: 15,
       avatarUrl: '/assets/temp-avatar.png',
       username: "User3"
     },
     {
       id: 1,
       setName: "Fizyka - wzory 123123",
       setCount: 15,
       avatarUrl: '/assets/temp-avatar.png',
       username: "User4"
     },
     {
       id: 1,
       setName: "Fizyka - wzoryasdasdas",
       setCount: 15,
       avatarUrl: '/assets/temp-avatar.png',
       username: "User5"
     },
     {
       id: 1,
       setName: "Fizyka - wzoryasdasdas",
       setCount: 15,
       avatarUrl: '/assets/temp-avatar.png',
       username: "User5"
     },
     {
       id: 1,
       setName: "Fizyka - wzoryasdasdas",
       setCount: 15,
       avatarUrl: '/assets/temp-avatar.png',
       username: "User5"
     }
  ]

  //Displaying more than 6 items on user button click 
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

  //doesnt work yet
  goToSet(setId: number): void {
    console.log("ASDASDA")
    this.router.navigate(['/set', setId]);
  }

}
