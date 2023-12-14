import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-flashcard',
  templateUrl: './flashcard.component.html',
  styleUrls: ['./flashcard.component.css']
})
export class FlashcardComponent implements OnChanges {
  @Input() term: string;
  @Input() definition: string;
  @Output() answerSelected = new EventEmitter<boolean>();

  displayedTerm: string;

  ngOnChanges(changes: SimpleChanges) {
    if (changes.term) {
      this.displayedTerm = changes.term.currentValue;
    }
  }

  checkAnswer() {
    this.answerSelected.emit(true);
  }

  showTerm() {
    this.displayedTerm = this.term;
  }

  toggleFlashcard() {
    if (this.displayedTerm === this.term) {
      this.displayedTerm = this.definition;
    } else {
      this.displayedTerm = this.term;
    }
  }

  goBack() {
    // Logic for going back
  }
}
