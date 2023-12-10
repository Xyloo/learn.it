import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LearningSetManagerComponent } from './learningSetManager.component';

describe('LearningSetManagerComponent', () => {
  let component: LearningSetManagerComponent;
  let fixture: ComponentFixture<LearningSetManagerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LearningSetManagerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LearningSetManagerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
