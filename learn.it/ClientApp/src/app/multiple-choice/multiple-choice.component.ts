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

  selectedOption: string | null = null;
  isCorrect: boolean | null = null;

  checkAnswer(selectedOption: string) {
    this.isCorrect = selectedOption === this.definition;
    this.selectedOption = selectedOption;
    console.log("isCorrect: " + this.isCorrect)

    setTimeout(() => {
      this.answerSelected.emit(this.isCorrect ?? false);
      this.isCorrect = null; 
    }, 1500);
  }

  goBack() {

  }

}
