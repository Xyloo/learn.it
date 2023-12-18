import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { StudySet } from '../models/study-sets/study-set';
import { StudySetsService } from '../services/study-sets/study-sets.service';
import { Flashcard } from '../models/flashcard';
import { FlashcardService } from '../services/flashcard/flashcard.service';


@Component({
  selector: 'app-learning-module',
  templateUrl: './learning-module.component.html',
  styleUrls: ['./learning-module.component.css']
})
export class LearningModuleComponent implements OnInit {
  currentMethod: string = 'multipleChoice';
  currentTerm: string = 'currentTerm';
  currentDefinition: string = 'currentDefinition';
  currentOptions: string[] = ['currentOptions', '2', '3', 'a'];
  availableMethods: string[] = ['multipleChoice', 'flashcard', 'input-quiz'];

  studySet: StudySet;
  flashcards: Flashcard[] = [];
  incorrectFlashcards: Flashcard[] = [];
  currentFlashcardIndex: number = 0;
  isReviewingIncorrect: boolean = false;
  quizCompleted: boolean = false;
  questionStartTime: Date;

  constructor(
    private studySetsService: StudySetsService,
    private flashcardService: FlashcardService,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) { }

  ngOnInit(): void {
    const studySetId = this.activatedRoute.snapshot.paramMap.get('id');
    if (!studySetId) return;
    this.questionStartTime = new Date();


    this.studySetsService.getStudySet(+studySetId).subscribe({
      next: (result) => {
        this.studySet = result;
        this.flashcards = result.flashcards;

        if (this.flashcards.length > 0) {
          this.setCurrentQuestion();
          this.generateRandomOptions();
        }
        else {  //brak fiszek
          this.currentTerm = '';
          this.currentDefinition = '';
        }
        if (this.flashcards.length < 3) {
          this.availableMethods = this.availableMethods.filter(method => method !== 'multipleChoice');
        }
        this.setLearningMethod();
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  generateRandomOptions() {
    const correctAnswer = this.flashcards[this.currentFlashcardIndex].definition;
    const otherOptions = this.flashcards
      .filter((_, index) => index !== this.currentFlashcardIndex)
      .map(fc => fc.definition)
      .sort(() => 0.5 - Math.random())
      .slice(0, 3);
    this.currentOptions = [correctAnswer, ...otherOptions].sort(() => 0.5 - Math.random());
  }

  setCurrentQuestion() {
    if (this.currentFlashcardIndex < this.flashcards.length) {
      let currentQuestion = this.flashcards[this.currentFlashcardIndex];
      this.currentTerm = currentQuestion.term;
      this.currentDefinition = currentQuestion.definition;
    }
    else if (this.incorrectFlashcards.length > 0 && !this.isReviewingIncorrect) {

      this.flashcards = this.incorrectFlashcards;
      this.incorrectFlashcards = [];
      this.currentFlashcardIndex = 0;
      this.isReviewingIncorrect = true;
      this.setCurrentQuestion();
    }
    else {
      this.quizCompleted = true;
      console.log("quizCompleted: " + this.quizCompleted)
    }
  }

  setLearningMethod() {
    const randomIndex = Math.floor(Math.random() * this.availableMethods.length);
    this.currentMethod = this.availableMethods[randomIndex];
    if (this.currentMethod === 'multipleChoice') {
      this.generateRandomOptions();
    }
  }

  onAnswerSelected(isCorrect: boolean) {
    const answerTime = new Date();
    const timeTaken = answerTime.getTime() - this.questionStartTime.getTime();
    if (this.currentFlashcardIndex < this.flashcards.length) {
      const currentFlashcard = this.flashcards[this.currentFlashcardIndex];
      if (currentFlashcard) {
        const currentFlashcardId = currentFlashcard.flashcardId;
        console.log("flashcardId: " + currentFlashcardId);
        this.flashcardService.generateAnswer({ answerTime: timeTaken, flashcardId: currentFlashcardId, isCorrect });

        if (!isCorrect) {
          this.incorrectFlashcards.push(currentFlashcard);
        }
      }
    }
    this.currentFlashcardIndex++;
    this.setCurrentQuestion();
    this.setLearningMethod();
  }
}


