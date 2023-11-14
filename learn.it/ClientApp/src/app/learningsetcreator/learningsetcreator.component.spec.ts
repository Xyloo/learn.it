import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LearningsetcreatorComponent } from './learningsetcreator.component';

describe('LearningsetcreatorComponent', () => {
  let component: LearningsetcreatorComponent;
  let fixture: ComponentFixture<LearningsetcreatorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LearningsetcreatorComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LearningsetcreatorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
