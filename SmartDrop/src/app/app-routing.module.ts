import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    loadChildren: () => import('./tabs/tabs.module').then(m => m.TabsPageModule)

  },
  {
    path: 'signup',
    loadChildren: () => import('./core/authentication/signup/signup.module').then( m => m.SignupPageModule)
  },
  {
    path: 'login',
    loadChildren: () => import('./core/authentication/login/login.module').then( m => m.LoginPageModule)
  },
  {
    path: 'statistics',
    loadChildren: () => import('./statistics/statistics.module').then( m => m.StatisticsPageModule)
  },
  {
    path: 'vegetables',
    loadChildren: () => import('./vegetables/vegetables.module').then( m => m.VegetablesPageModule)
  },
  // {
  //   path: 'myaccount',
  //   loadChildren: () => import('./myaccount/myaccount.module').then( m => m.MyaccountPageModule)
  // }

];
@NgModule({
  imports: [
    RouterModule.forRoot(routes, { preloadingStrategy: PreloadAllModules })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {}
