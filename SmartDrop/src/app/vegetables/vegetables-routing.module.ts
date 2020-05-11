import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { VegetablesPage } from './vegetables.page';

const routes: Routes = [
  {
    path: '',
    component: VegetablesPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class VegetablesPageRoutingModule {}
