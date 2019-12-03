import { IonicModule } from '@ionic/angular';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SchedulePage } from './schedule.page';
import { ModalZoneComponent } from './modal-zone/modal-zone.component';

@NgModule({
  imports: [
    IonicModule,
    CommonModule,
    FormsModule,
    RouterModule.forChild([{ path: '', component: SchedulePage }])
  ],
  declarations: [
    SchedulePage,
    ModalZoneComponent
  ],
  entryComponents:
  [
    ModalZoneComponent
  ]
})
export class SchedulePageModule {}
