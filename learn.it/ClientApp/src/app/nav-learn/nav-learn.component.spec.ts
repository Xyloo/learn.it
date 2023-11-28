import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NavLearnComponent } from './nav-learn.component';

describe('NavLearnComponent', () => {
  let component: NavLearnComponent;
  let fixture: ComponentFixture<NavLearnComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NavLearnComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NavLearnComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
