import { TestBed } from '@angular/core/testing';

import { MyAccountService } from './my-account.service';

describe('MyAccountService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: MyAccountService = TestBed.get(MyAccountService);
    expect(service).toBeTruthy();
  });
});
