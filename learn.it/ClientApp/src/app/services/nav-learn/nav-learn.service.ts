import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NavLearnService {

  private flashcardSource = new BehaviorSubject<number>(0);
  private totalFlashcardSource = new BehaviorSubject<number>(0);
  private studySetNameSoruce = new BehaviorSubject<string>("");
  private selectedMethodSource = new BehaviorSubject<string[]>(['multipleChoice', 'flashcard', 'input-quiz']); 

  currentFlashcard = this.flashcardSource.asObservable();
  totalFlashcards = this.totalFlashcardSource.asObservable();
  currentStudySetName = this.studySetNameSoruce.asObservable();
  selectedMethods = this.selectedMethodSource.asObservable();


  setSelectedMethods(methods: string[]) {
    this.selectedMethodSource.next(methods);
  }

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
    if(this.currentFlashcard < this.totalFlashcards)
      this.flashcardSource.next(this.flashcardSource.value + 1);
  }

}
