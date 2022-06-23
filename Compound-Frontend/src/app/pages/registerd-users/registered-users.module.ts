import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { LayoutModule } from 'src/app/shared/layout/layout.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { RegisteredUsersComponent } from './registered-users/registered-users.component'
import { FormOwnerRegistrationComponent } from './form-owner-registration/form-owner-registration.component';
import { HttpClientJsonpModule, HttpClientModule } from '@angular/common/http';
import { UploadFileModule } from 'src/app/shared/upload-file/upload-file.module';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
@NgModule({
  declarations: [RegisteredUsersComponent, FormOwnerRegistrationComponent],
  imports: [
    SharedModule,
    FormLayoutModule,
    LayoutModule,
    HttpClientJsonpModule,
    HttpClientModule,
    MatProgressSpinnerModule,
    UploadFileModule,
    MatAutocompleteModule,
    RouterModule.forChild([
      { path: '', component: RegisteredUsersComponent },
      { path: '**', pathMatch: 'full', redirectTo: "registeredUsers" }
    ]),
  ]
})
export class RegisteredUsersModule { }
