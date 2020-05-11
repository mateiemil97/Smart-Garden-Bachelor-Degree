import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { LiveStreamPage } from './live-stream.page';

const routes: Routes = [
  {
    path: '',
    component: LiveStreamPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class LiveStreamPageRoutingModule {}
