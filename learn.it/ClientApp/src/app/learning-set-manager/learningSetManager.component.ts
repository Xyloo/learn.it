import { Component, ElementRef, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-learningSetManager',
  templateUrl: './learningSetManager.component.html',
  styleUrls: ['./learningSetManager.component.css']
})
export class LearningSetManagerComponent {
  @ViewChild('newFlashcard') newFlashcardElement: ElementRef | undefined;
  @ViewChild('flashcardContainer') flashcardContainerElement: ElementRef | undefined;
  constructor(private location: Location, private router: Router, private route: ActivatedRoute) { }

  isEditMode: boolean = false;

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      if (params.has('id')) {
        this.isEditMode = true;
        const id = params.get('id');

          //set service - load data, fill title, desc, flashcards it
      } else {
        this.isEditMode = false;        
      }
    });
  }


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
    if (this.flashcards.length>1)
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

  saveSet() {
    //set creation logic
  }

}
