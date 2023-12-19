import { Component } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-choose-method-dialog',
  templateUrl: './choose-method-dialog.component.html',
  styleUrls: ['./choose-method-dialog.component.css']
})
export class ChooseMethodDialogComponent {
  multipleChoiceSelected: boolean = true;
  flashcardSelected: boolean = true;
  inputQuizSelected: boolean = true;


  constructor(
    public dialogRef: MatDialogRef<ChooseMethodDialogComponent>
  ) { }
  
  saveAndCloseDialog(): void {
    const selectedMethods = this.getSelectedMethods();
    if (selectedMethods && selectedMethods.length > 0) {
      this.dialogRef.close(selectedMethods);
    } else {
      alert('Proszę wybrać przynajmniej jeden tryb nauki.');
    }
  }

  getSelectedMethods(): string[] {
    let selectedMethods = [];
    if (this.multipleChoiceSelected) selectedMethods.push('multipleChoice');
    if (this.flashcardSelected) selectedMethods.push('flashcard');
    if (this.inputQuizSelected) selectedMethods.push('input-quiz');
    return selectedMethods;
  }
}
