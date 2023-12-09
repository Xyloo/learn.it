import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChooseGroupDialogComponent } from './choose-group-dialog.component';

describe('ChooseGroupDialogComponent', () => {
  let component: ChooseGroupDialogComponent;
  let fixture: ComponentFixture<ChooseGroupDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChooseGroupDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChooseGroupDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
