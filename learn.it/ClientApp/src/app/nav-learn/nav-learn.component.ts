import { Component } from '@angular/core';
import { Location } from '@angular/common';

@Component({
  selector: 'app-nav-learn',
  templateUrl: './nav-learn.component.html',
  styleUrls: ['./nav-learn.component.css']
})
export class NavLearnComponent {
  currentItem: number = 2;
  totalItems: number = 5;
  learningSetTitle: string = 'Learn'; //get from service

  constructor(private location: Location) { }

  getProgressWidth(): string {
    const progress = (this.currentItem / this.totalItems) * 100;
    return `${progress}%`;
  }

  goBack() {
    //this.location.back();
  }

}
