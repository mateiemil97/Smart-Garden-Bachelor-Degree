import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { MyaccountPageRoutingModule } from './myaccount-routing.module';

import { MyaccountPage } from './myaccount.page';
import { AddIrrigationSystemModalComponent } from './add-irrigation-system-modal/add-irrigation-system-modal.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    MyaccountPageRoutingModule,
  ],
  declarations: [MyaccountPage, AddIrrigationSystemModalComponent],
  entryComponents: [AddIrrigationSystemModalComponent]
})
export class MyaccountPageModule {}
