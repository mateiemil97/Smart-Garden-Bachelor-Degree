import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { AddIrrigationSystemModalComponent } from './add-irrigation-system-modal.component';

describe('AddIrrigationSystemModalComponent', () => {
  let component: AddIrrigationSystemModalComponent;
  let fixture: ComponentFixture<AddIrrigationSystemModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddIrrigationSystemModalComponent ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(AddIrrigationSystemModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
