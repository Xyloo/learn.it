import { TestBed } from '@angular/core/testing';

import { NavLearnService } from './nav-learn.service';

describe('NavLearnService', () => {
  let service: NavLearnService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NavLearnService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
