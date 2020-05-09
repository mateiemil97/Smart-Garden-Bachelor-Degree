import { TestBed } from '@angular/core/testing';

import { GlobalVegetablesService } from './global-vegetables.service';

describe('GlobalVegetablesService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: GlobalVegetablesService = TestBed.get(GlobalVegetablesService);
    expect(service).toBeTruthy();
  });
});
