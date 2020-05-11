import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { LiveStreamPage } from './live-stream.page';

describe('LiveStreamPage', () => {
  let component: LiveStreamPage;
  let fixture: ComponentFixture<LiveStreamPage>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LiveStreamPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(LiveStreamPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
