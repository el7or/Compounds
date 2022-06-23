import { RoleActionGuard } from './services/api/login/role-action.guard';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginGuard } from './services/api/login/login.guard';
import { SystemPageActions } from './services/system-page-actions.enum';

const routes: Routes = [
  {
    path: ':compoundId/dashboard',
    canActivate: [LoginGuard],
    loadChildren: () => import('./pages/dashboard/dashboard.module').then(m => m.DashboardModule)
  },
  {
    path: 'auth',
    loadChildren: () => import('./pages/auth/login.module').then(m => m.LoginModule)
  },
  {
    path: 'accounts',
    loadChildren: () => import('./pages/accounts/accounts.module').then(m => m.AccountsModule)
  },
  {
    path: ':compoundId/units',
    canActivate: [LoginGuard],
    loadChildren: () => import('./pages/units/units.module').then(m => m.UnitsModule)
  },
  {
    path: ':compoundId/gates',
    canActivate: [LoginGuard],
    loadChildren: () => import('./pages/gates/gates.module').then(m => m.GatesModule)
  },
  {
    path: 'managementUsers',
    canActivate: [LoginGuard],
    loadChildren: () => import('./pages/subscriptions/subscriptions.module').then(m => m.SubscriptionsModule)
  },
  {
    path: 'managementRoles',
    canActivate: [LoginGuard],
    loadChildren: () => import('./pages/roles/roles.module').then(m => m.RolesModule)
  },
  {
    path: ':compoundId/services',
    loadChildren: () => import('./pages/services/services.module').then(m => m.ServicesModule)
  },
  {
    path: ':compoundId/issues',
    loadChildren: () => import('./pages/issues/issues.module').then(m => m.IssuesModule)
  },
  {
    path: ':compoundId/owners',
    loadChildren: () => import('./pages/owners/owners.module').then(m => m.OwnersModule)
  },
  {
    path: ':compoundId/news',
    loadChildren: () => import('./pages/news/news.module').then(m => m.NewsModule),
    canActivate: [RoleActionGuard],
    data: {
      roleActions: [
        SystemPageActions.NewsDisplay
      ]
    }
  },
  {
    path: ':compoundId/ads',
    loadChildren: () => import('./pages/ads/ads.module').then(m => m.AdsModule)
  },
  {
    path: ':compoundId/notifications',
    loadChildren: () => import('./pages/notifications/notifications.module').then(m => m.NotificationsModule),
  },
  {
    path: ':compoundId/visits',
    loadChildren: () => import('./pages/visits/visits.module').then(m => m.VisitsModule)
  },
  {
    path: 'compounds',
    canActivate: [LoginGuard],
    loadChildren: () => import('./pages/compound/compounds.module').then(m => m.CompoundsModule)
  },
  {
    path: 'registeredUsers',
    canActivate: [LoginGuard],
    loadChildren: () => import('./pages/registerd-users/registered-users.module').then(m => m.RegisteredUsersModule)
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: "/compounds"
  },

  // {
  //   path: '404',
  //   component: NotFoundComponent
  // },

  // {
  //   path: '**',
  //   redirectTo: '404'
  // }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
