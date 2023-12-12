import { Component, ElementRef, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { ChooseGroupDialogComponent } from '../choose-group-dialog/choose-group-dialog.component';
import { StudySetsService } from '../services/study-sets/study-sets.service';
import { NgForm } from '@angular/forms';
import { Visibility } from '../models/study-sets/study-set-visibility.dto';
import { Flashcard } from '../models/flashcard';
import { FlashcardService } from '../services/flashcard/flashcard.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { StudySet } from '../models/study-sets/study-set';


@Component({
  selector: 'app-learningSetManager',
  templateUrl: './learningSetManager.component.html',
  styleUrls: ['./learningSetManager.component.css']
})
export class LearningSetManagerComponent {
  @ViewChild('newFlashcard') newFlashcardElement: ElementRef | undefined;
  @ViewChild('flashcardContainer') flashcardContainerElement: ElementRef | undefined;
  @ViewChild('form') form!: NgForm;

  studySet: StudySet = {
    id: -1,
    name: '',
    description: '',
    visibility: 0,
    group: -1,
    flashcards: [],
    flashcardsCount: 0,
    creator: {
      username: '',
      avatar: null
    }
  };

  showAddError = false;
  flashcards: Flashcard[] = [
    { flashcardId: 1, term: 'Wpisz dane', definition: 'Wpisz dane', isTermText: true }
  ];

  originalFlashcards: Flashcard[] = [];
  initialSetData: StudySet;

  nextId = 2; 

  constructor(
    private location: Location,
    private router: Router,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private studySetsService: StudySetsService,
    private flashcardService: FlashcardService,
    private snackBar: MatSnackBar
  ) { }

  isEditMode: boolean = false;

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      if (params.has('id')) {
        const setIdParam = params.get('id');
        this.isEditMode = true;
        this.studySet.id = Number(setIdParam);

        this.studySetsService.getStudySet(this.studySet.id).subscribe(set => {
          this.studySet.name = set.name;
          this.studySet.description = set.description;
          this.studySet.visibility = set.visibility;
          this.flashcards = set.flashcards;
          this.originalFlashcards = JSON.parse(JSON.stringify(set.flashcards));
          this.initialSetData = JSON.parse(JSON.stringify(set));
        });
      }
      else {
        this.isEditMode = false;
      }
    });
  }

  addFlashcard() {
    if (this.canAddFlashcard()) {
      const newId = this.getMaxId() + 1; 
      this.flashcards.push({ flashcardId: newId, term: '', definition: '', isTermText: true });
      this.showAddError = false;
      setTimeout(() => {
        if (this.newFlashcardElement) {
          this.newFlashcardElement.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'end' });
        }
      });
    }
    else
      this.showAddError = true;
  }


  deleteFlashcard(flashcardId: number) {
    if (this.flashcards.length > 1)
      this.flashcards = this.flashcards.filter(flashcard => flashcard.flashcardId !== flashcardId);

    if (this.isEditMode) {
      this.flashcardService.deleteFlashcard(flashcardId).subscribe({
        next: () => {
          this.openSnackBar("Fiszka została pomyślnie usunięta.", "Zamknij");
        },
        error: () => {
          this.openSnackBar("Wystąpił błąd podczas usuwania fiszki.", "Zamknij");
        }
      });
    }
  }

  getMaxId() {
    return this.flashcards.reduce((max, flashcard) => flashcard.flashcardId > max ? flashcard.flashcardId : max, 0);
  }

  canAddFlashcard(): boolean {
    const lastFlashcard = this.flashcards[this.flashcards.length - 1];
    return lastFlashcard.term.trim() !== '' && lastFlashcard.definition.trim() !== '';
  }
  goBack() {
    this.location.back();
  }

  saveSet() {

    if (!this.form.valid) {
      this.openSnackBar("Wypełnij wszystkie wymagane pola", "Zamknij");
      return;
    }

    if (this.isEditMode) {
      const { added, modified } = this.getFlashcardChanges();
      added.forEach(flashcard => {
        var backendFlashcard = {
          Term: flashcard.term,
          Definition: flashcard.definition,
          StudySetId: this.studySet.id
        };
        this.flashcardService.createFlashcards([backendFlashcard]);
      });

      modified.forEach(flashcard => {
        var backendFlashcard = {
          Term: flashcard.term,
          Definition: flashcard.definition,
          StudySetId: this.studySet.id
        };
        this.flashcardService.updateFlashcard(flashcard.flashcardId, backendFlashcard).subscribe({
          next: results => {
            this.openSnackBar("Zestaw został pomyślnie zapisany", "Zamknij");
          },
          error: error => {
            this.openSnackBar("Wystąpił błąd podczas zapisu zestawu", "Zamknij");
          }
        });
      });
      if (JSON.stringify(this.initialSetData) !== JSON.stringify(this.studySet)) {

        var studySetDto = {
          name: this.studySet.name,
          description: this.studySet.description,
          visibility: this.studySet.visibility,
          groupId: this.studySet.group == -1 ? undefined : this.studySet.group
        };
        this.studySetsService.updateStudySet(this.studySet.id, studySetDto).subscribe({
          next: results => {
            this.openSnackBar("Zestaw został pomyślnie zapisany", "Zamknij");
          },
          error: error => {
            this.openSnackBar("Wystąpił błąd podczas zapisu zestawu", "Zamknij");
          }
        });;
      }
      //this.router.navigate(['/']);
    }

    else {
      const newStudySet = {
        name: this.studySet.name,
        description: this.studySet.description,
        visibility: this.studySet.visibility,
        groupId: this.studySet.group == -1 ? null : this.studySet.group
      };

      var createdSetId = -1;


      this.studySetsService.createStudySet(newStudySet).subscribe({
        next: (response) => {
          createdSetId = response.studySetId;
          console.log("created set id: " + createdSetId);
          const flashcardsForBackend = this.flashcards.map(flashcard => ({
            term: flashcard.term,
            definition: flashcard.definition,
            studySetId: createdSetId
          }));

          this.createFlashcards(flashcardsForBackend);
        },
        error: (error) => {
          this.openSnackBar("Wystąpił błąd podczas tworzenia zestawu", "Zamknij");
        }
      });
    }
  }

  createFlashcards(flashcards: any[]) {

    this.flashcardService.createFlashcards(flashcards).subscribe({
      next: results => {
        this.openSnackBar("Zestaw został utworzony pomyślnie", "Zamknij");
        this.router.navigate(['/']);
      },
      error: error => {
        console.error('Error creating flashcards', error);
        this.openSnackBar("Wystąpił błąd podczas tworzenia zestawu", "Zamknij");
      }
    });
  }

  getFlashcardChanges() {
    const added = this.flashcards.filter(fc => !fc.flashcardId);
    const modified = this.flashcards.filter(fc => {
      if (!fc.flashcardId) return false;
      const original = this.originalFlashcards.find(ofc => ofc.flashcardId === fc.flashcardId);
      return original && (fc.term !== original.term || fc.definition !== original.definition);
    });
    return { added, modified };
  }



  openSnackBar(message: string, action: string = 'Zamknij') {
    this.snackBar.open(message, action, {
      duration: 2000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
    });
  }

  openDialog(): void {
    const dialogRef = this.dialog.open(ChooseGroupDialogComponent, {
      width: '260px',
      data: { setId: this.studySet.id, lerningSetVisibility: this.studySet.visibility }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.studySet.group = result.groupId;
        this.studySet.visibility = result.visibility;
      }
    });
  }
}
