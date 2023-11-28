import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-input-quiz',
  templateUrl: './input-quiz.component.html',
  styleUrls: ['./input-quiz.component.css']
})
export class InputQuizComponent {
  @Input() term: string;
  @Input() definition: string;
  @Input() options: string[];
  @Output() answerSelected = new EventEmitter<boolean>();

  checkAnswer(selectedOption: string) {
    const isCorrect = selectedOption === this.definition;
    this.answerSelected.emit(isCorrect);
  }

  goBack() {

  }

  markAsFavourite() {

  }
}
