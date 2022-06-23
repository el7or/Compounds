import { NgModule } from '@angular/core'; 
import { SubscriptionComponent } from './subscription/subscription.component';
import { RouterModule } from '@angular/router';
import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { LayoutModule } from 'src/app/shared/layout/layout.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { CompanyUserComponent } from './company-user/company.user.component';
import { UploadFileModule } from 'src/app/shared/upload-file/upload-file.module';

@NgModule({
  declarations: [SubscriptionComponent,CompanyUserComponent],
  imports: [
    SharedModule,
    FormLayoutModule,
    LayoutModule,
    UploadFileModule,
    RouterModule.forChild([
      { path: '', component: SubscriptionComponent }, 
      { path: '**', pathMatch: 'full', redirectTo: "subscription" }
    ]),
  ]
})
export class SubscriptionsModule { }
