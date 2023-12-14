import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-input-quiz',
  templateUrl: './input-quiz.component.html',
  styleUrls: ['./input-quiz.component.css']
})
export class InputQuizComponent implements OnChanges {
  @Input() term: string;
  @Input() definition: string;
  @Input() options: string[];
  @Output() answerSelected = new EventEmitter<boolean>();
  userInput: string = '';

  ngOnChanges(changes: SimpleChanges): void {
    this.userInput = ''
  }

  checkAnswer(selectedOption: string) {
    const isCorrect = selectedOption === this.definition;
    this.answerSelected.emit(isCorrect);
  }
  skipAnswer() {
    this.answerSelected.emit(false);
  }

  goBack() {

  }

  markAsFavourite() {

  }
}
