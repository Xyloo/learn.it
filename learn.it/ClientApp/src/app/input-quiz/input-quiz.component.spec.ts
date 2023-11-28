import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputQuizComponent } from './input-quiz.component';

describe('InputQuizComponent', () => {
  let component: InputQuizComponent;
  let fixture: ComponentFixture<InputQuizComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InputQuizComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InputQuizComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
