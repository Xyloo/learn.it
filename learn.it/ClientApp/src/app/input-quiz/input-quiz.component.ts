import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FlashcardService } from '../services/flashcard/flashcard.service';
import { Flashcard } from '../models/flashcard';

@Component({
  selector: 'app-input-quiz',
  templateUrl: './input-quiz.component.html',
  styleUrls: ['./input-quiz.component.css']
})
export class InputQuizComponent implements OnChanges {

  constructor(
    private flashcardService: FlashcardService
  ){ }

  @Input() flashcard: Flashcard;
  @Input() options: string[];
  @Output() answerSelected = new EventEmitter<boolean>();
  userInput: string = '';
  needsMoreRepetitions: boolean = false;

  ngOnChanges(changes: SimpleChanges): void {
    this.userInput = ''
    if (changes['flashcard'] && this.flashcard.flashcardId !== -1) {
      this.flashcardService.getNeedsMoreRepetitions(this.flashcard.flashcardId).subscribe(
        data => this.needsMoreRepetitions = data
      );
    }
  }

  checkAnswer(selectedOption: string) {
    const isCorrect = selectedOption === this.flashcard.definition;
    this.answerSelected.emit(isCorrect);
  }
  skipAnswer() {
    this.answerSelected.emit(false);
  }

  goBack() {

  }

  markAsDifficult() {
    this.needsMoreRepetitions = !this.needsMoreRepetitions;
    this.flashcardService.markAsDifficult(this.flashcard.flashcardId, this.needsMoreRepetitions).subscribe(
      data => this.needsMoreRepetitions = data.needsMoreRepetitions
    );
  }
}
