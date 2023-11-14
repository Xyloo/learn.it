import { Component, ElementRef, ViewChild } from '@angular/core';
import { Location } from '@angular/common';

@Component({
  selector: 'app-learningsetcreator',
  templateUrl: './learningsetcreator.component.html',
  styleUrls: ['./learningsetcreator.component.css']
})
export class LearningsetcreatorComponent {
  @ViewChild('newFlashcard') newFlashcardElement: ElementRef | undefined;
  @ViewChild('flashcardContainer') flashcardContainerElement: ElementRef | undefined;
  constructor(private location: Location) { }

  showAddError = false;
  flashcards = [
    { id: 1, term: 'lorem ipsum', definition: 'lorem ipsum' }
  ];
  nextId = 2; //hardcoded for now

  learningSetTitle: string = '';

  addFlashcard() {
    if (this.canAddFlashcard()) {
      const newId = this.getMaxId() + 1; // Get the current max id and add 1
      this.flashcards.push({ id: newId, term: '', definition: '' });
      this.showAddError = false;
      setTimeout(() => {
        if (this.newFlashcardElement) {
          this.newFlashcardElement.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'end' });
        }
      });
    }
    else
      this.showAddError = true;
}


  deleteFlashcard(flashcardId: number) {
    this.flashcards = this.flashcards.filter(flashcard => flashcard.id !== flashcardId);
  }

  getMaxId() {
    return this.flashcards.reduce((max, flashcard) => flashcard.id > max ? flashcard.id : max, 0);
  }

  canAddFlashcard(): boolean {
    const lastFlashcard = this.flashcards[this.flashcards.length - 1];
    return lastFlashcard.term.trim() !== '' && lastFlashcard.definition.trim() !== '';
  }
  goBack() {
    this.location.back();
  }

  createSet() {
    //set creation logic
  }

}
