 import { NgModule } from '@angular/core';
import { DashboardComponent } from './dashboard.component';
import { LayoutModule } from 'src/app/shared/layout/layout.module';
import { RouterModule } from '@angular/router';
import { SharedModule } from 'src/app/shared/shared.module';
import { GoogleChartsModule } from 'angular-google-charts';

@NgModule({
  declarations: [DashboardComponent],
  imports: [
    RouterModule.forChild([{ path: '', component: DashboardComponent }]),
    LayoutModule,
    SharedModule,
    GoogleChartsModule
  ]
})
export class DashboardModule { }
