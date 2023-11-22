import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-flashcard',
  templateUrl: './flashcard.component.html',
  styleUrls: ['./flashcard.component.css']
})
export class FlashcardComponent {
  @Input() term: string;
  @Input() definition: string;
  @Output() answerSelected = new EventEmitter<boolean>();

  checkAnswer(selectedOption: string) {
    const isCorrect = selectedOption === this.definition; 
    this.answerSelected.emit(isCorrect);
  }

  goBack() {

  }

}
