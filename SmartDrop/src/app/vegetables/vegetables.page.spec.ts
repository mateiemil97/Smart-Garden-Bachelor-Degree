import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { VegetablesPage } from './vegetables.page';

describe('VegetablesPage', () => {
  let component: VegetablesPage;
  let fixture: ComponentFixture<VegetablesPage>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VegetablesPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(VegetablesPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
