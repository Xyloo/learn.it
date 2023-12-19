import { Component, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { NavLearnService } from '../services/nav-learn/nav-learn.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-nav-learn',
  templateUrl: './nav-learn.component.html',
  styleUrls: ['./nav-learn.component.css']
})
export class NavLearnComponent implements OnDestroy {
  currentItem: number = 2;
  totalItems: number = 5;
  learningSetTitle: string = 'Learn'; //get from service
  private subscriptions = new Subscription();

  constructor(
    private router: Router,
    private learnService: NavLearnService
  ) {
    this.subscriptions.add(this.learnService.currentFlashcard.subscribe(item => this.currentItem = item));
    this.subscriptions.add(this.learnService.totalFlashcards.subscribe(items => this.totalItems = items));
    this.subscriptions.add(this.learnService.currentStudySetName.subscribe(items => this.learningSetTitle = items));
  }
    ngOnDestroy(): void {
      this.subscriptions.unsubscribe();
    }

  getProgressWidth(): string {
    const progress = (this.currentItem / this.totalItems) * 100;
    return `${progress}%`;
  }

  goBack() {
    this.router.navigate(['']);
  }

}
