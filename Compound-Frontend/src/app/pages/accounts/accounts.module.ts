import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { AccountActivationComponent as ActivationSuccessfulComponent } from './account-activation/account-activation.component';
import { AccountCreationComponent as ActivationRequestComponent } from './account-creation/account-creation.component';
import { FormAccountComponent } from './form-account/form-account.component';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ReplyLayoutModule } from 'src/app/shared/reply-layout/reply-layout.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { CompaniesService } from 'src/app/services/api/admin/companies.service';
import { UploadFileModule } from 'src/app/shared/upload-file/upload-file.module';
import { GoogleMapsModule } from '@angular/google-maps';
import { HttpClientModule, HttpClientJsonpModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { PhoneFormModule } from 'src/app/shared/phone-form/phone-form.module';

@NgModule({
  declarations: [FormAccountComponent, ActivationRequestComponent, ActivationSuccessfulComponent],
  imports: [
    CommonModule,
    SharedModule, 
    ReplyLayoutModule,
    FormLayoutModule,
    HttpClientJsonpModule,
    HttpClientModule,
    UploadFileModule,
    GoogleMapsModule,
    PhoneFormModule,
    
    RouterModule.forChild([
      { path: 'new/:planId', component: FormAccountComponent },
      { path: 'activation-request', component: ActivationRequestComponent },
      { path: 'successful/:token', component: ActivationSuccessfulComponent },
    ]),
  ],
  providers: [CompaniesService]
})
export class AccountsModule { }
