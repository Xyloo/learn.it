import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { Flashcard } from '../models/flashcard';
import { FlashcardService } from '../services/flashcard/flashcard.service';

@Component({
  selector: 'app-multiple-choice',
  templateUrl: './multiple-choice.component.html',
  styleUrls: ['./multiple-choice.component.css']
})
export class MultipleChoiceComponent implements OnChanges {

  @Input() flashcard: Flashcard;
  @Input() options: string[];
  @Output() answerSelected = new EventEmitter<boolean>();

  selectedOption: string | null = null;
  isCorrect: boolean | null = null;
  needsMoreRepetitions: boolean = false;

  constructor(private flashcardService: FlashcardService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['flashcard'] && this.flashcard.flashcardId !== -1) {
      this.flashcardService.getNeedsMoreRepetitions(this.flashcard.flashcardId).subscribe(
        data => this.needsMoreRepetitions = data
      );
    }
  }

  checkAnswer(selectedOption: string) {
    this.isCorrect = selectedOption === this.flashcard.definition;
    this.selectedOption = selectedOption;

    setTimeout(() => {
      this.answerSelected.emit(this.isCorrect ?? false);
      this.isCorrect = null; 
    }, 1500);
  }

  markAsDifficult() {
    this.needsMoreRepetitions = !this.needsMoreRepetitions;
    this.flashcardService.markAsDifficult(this.flashcard.flashcardId, this.needsMoreRepetitions).subscribe(
      data => {
        this.needsMoreRepetitions = data.needsMoreRepetitions;
      }
    );
  }

  goBack() {

  }

}
