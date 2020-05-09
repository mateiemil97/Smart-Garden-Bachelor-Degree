import { TestBed } from '@angular/core/testing';

import { VegetablesServiceService } from './vegetables-service.service';

describe('VegetablesServiceService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: VegetablesServiceService = TestBed.get(VegetablesServiceService);
    expect(service).toBeTruthy();
  });
});
