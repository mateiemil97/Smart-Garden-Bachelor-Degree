import { IonicModule } from '@ionic/angular';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SchedulePage } from './schedule.page';
import { ModalZonePage } from './modal-zone/modal-zone.page';
import { EditMoistureModalComponent } from './edit-moisture-modal/edit-moisture-modal.component';
@NgModule({
  imports: [
    IonicModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild([{ path: '', component: SchedulePage }])
  ],
  declarations: [
    SchedulePage,
    ModalZonePage,
    EditMoistureModalComponent
  ],
  entryComponents:
  [
    ModalZonePage,
    EditMoistureModalComponent
  ]
})
export class SchedulePageModule {}
