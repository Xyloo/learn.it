import { Component } from '@angular/core';


@Component({
  selector: 'app-learning-module',
  templateUrl: './learning-module.component.html',
  styleUrls: ['./learning-module.component.css']
})
export class LearningModuleComponent {
  currentMethod: string = 'input-quiz';
  currentTerm: string = 'currentTerm';
  currentDefinition: string = 'currentDefinition';
  currentOptions: string[] = ['currentOptions', '2', '3', 'a'];




  onAnswerSelected(isCorrect: boolean) {
    if (isCorrect) {
      // Handle correct answer
    } else {
      // Handle incorrect answer
    }
  }
}


