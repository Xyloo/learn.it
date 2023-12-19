import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { Flashcard } from '../models/flashcard';
import { FlashcardService } from '../services/flashcard/flashcard.service';

@Component({
  selector: 'app-flashcard',
  templateUrl: './flashcard.component.html',
  styleUrls: ['./flashcard.component.css']
})
export class FlashcardComponent implements OnChanges {
  @Input() flashcard: Flashcard;
  @Output() answerSelected = new EventEmitter<boolean>();

  needsMoreRepetitions: boolean | null = null;
  displayedTerm: string;

  constructor(private flashcardService: FlashcardService) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['flashcard'] && this.flashcard.flashcardId !== -1) {
      //this.displayedTerm = changes.flashcard.currentValue;
      this.flashcardService.getNeedsMoreRepetitions(this.flashcard.flashcardId).subscribe(
        data => this.needsMoreRepetitions = data
      );
    }
  }

  checkAnswer() {
    this.answerSelected.emit(true);
  }

  showTerm() {
    this.displayedTerm = this.flashcard.term;
  }

  toggleFlashcard() {
    if (this.displayedTerm === this.flashcard.term) {
      this.displayedTerm = this.flashcard.definition;
    } else {
      this.displayedTerm = this.flashcard.term;
    }
  }

  markAsDifficult() {
    this.needsMoreRepetitions = !this.needsMoreRepetitions;
    this.flashcardService.markAsDifficult(this.flashcard.flashcardId, this.needsMoreRepetitions).subscribe(
      data => this.needsMoreRepetitions = data
    );
  }

  goBack() {
    // Logic for going back
  }
}
