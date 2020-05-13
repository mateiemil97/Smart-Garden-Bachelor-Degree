import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { VegetablesPageRoutingModule } from './vegetables-routing.module';

import { VegetablesPage } from './vegetables.page';
import { AddVegetablesModalComponent } from './add-vegetables-modal/add-vegetables-modal.component';
import { GlobalVegetablesComponent } from '../global-vegetables/global-vegetables.component';
import {MatExpansionModule} from '@angular/material/expansion';
import { EditMoistureModalComponent } from '../schedule/edit-moisture-modal/edit-moisture-modal.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    VegetablesPageRoutingModule,
    MatExpansionModule
  ],
  declarations: [VegetablesPage,
     AddVegetablesModalComponent,
     GlobalVegetablesComponent,
    ],
  entryComponents: [AddVegetablesModalComponent, GlobalVegetablesComponent]
})
export class VegetablesPageModule {}
