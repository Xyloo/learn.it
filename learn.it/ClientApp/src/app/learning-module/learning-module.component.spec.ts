import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LearningModuleComponent } from './learning-module.component';
import {  }

describe('LearningModuleComponent', () => {
  let component: LearningModuleComponent;
  let fixture: ComponentFixture<LearningModuleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LearningModuleComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LearningModuleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
