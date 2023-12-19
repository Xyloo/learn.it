import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NavLearnService {

  private flashcardSource = new BehaviorSubject<number>(0);
  private totalFlashcardSource = new BehaviorSubject<number>(0);
  private studySetNameSoruce = new BehaviorSubject<string>("");

  currentFlashcard = this.flashcardSource.asObservable();
  totalFlashcards = this.totalFlashcardSource.asObservable();
  currentStudySetName = this.studySetNameSoruce.asObservable();

  setCurrentItem(item: number) {
    this.flashcardSource.next(item);
  }

  setTotalItems(items: number) {
    this.totalFlashcardSource.next(items);
  }

  setStudySetName(name: string) {
    this.studySetNameSoruce.next(name);
  }

  incrementCurrentItem() {
    this.flashcardSource.next(this.flashcardSource.value + 1);
  }

}
