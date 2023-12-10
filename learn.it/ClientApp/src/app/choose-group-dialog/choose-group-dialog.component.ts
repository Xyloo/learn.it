import { Component } from '@angular/core';

@Component({
  selector: 'app-choose-group-dialog',
  templateUrl: './choose-group-dialog.component.html',
  styleUrls: ['./choose-group-dialog.component.css']
})
export class ChooseGroupDialogComponent {
  groupId: string = '';
  visibility: boolean = false;

  constructor() { }
}
