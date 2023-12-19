import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChooseMethodDialogComponent } from './choose-method-dialog.component';

describe('ChooseMethodDialogComponent', () => {
  let component: ChooseMethodDialogComponent;
  let fixture: ComponentFixture<ChooseMethodDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChooseMethodDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChooseMethodDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
