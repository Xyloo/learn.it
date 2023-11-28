import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-multiple-choice',
  templateUrl: './multiple-choice.component.html',
  styleUrls: ['./multiple-choice.component.css']
})
export class MultipleChoiceComponent {
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

}
