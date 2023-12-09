import { TestBed } from '@angular/core/testing';

import { StudySetsService } from './study-sets.service';

describe('StudySetsService', () => {
  let service: StudySetsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StudySetsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
