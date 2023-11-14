import { Component } from '@angular/core';
import { Location } from '@angular/common';
import { SearchResultDto } from '../models/searchesult.dto';


@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent {
  constructor(private location: Location) { }
  searchedText: string = '';

   searchResults: SearchResultDto[] = [
    {
       id: 1,
       setCount: 25,
       avatarUrl: 'asd',
       username: "User1" 
     },
     {
       id: 1,
       setCount: 5,
       avatarUrl: 'asd',
       username: "User2"
     },
     {
       id: 1,
       setCount: 15,
       avatarUrl: 'asd',
       username: "User3"
     },
     {
       id: 1,
       setCount: 15,
       avatarUrl: 'asd',
       username: "User4"
     },
     {
       id: 1,
       setCount: 15,
       avatarUrl: 'asd',
       username: "User5"
     }
  ]



  goBack() {
    this.location.back();
  }
}
