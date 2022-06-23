import { NgModule } from '@angular/core';
import { RoleListComponent } from './role-list/role-list.component';
import { RoleFormComponent } from './role-form/role-form.component';
import { RouterModule } from '@angular/router';
import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { LayoutModule } from 'src/app/shared/layout/layout.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { UploadFileModule } from 'src/app/shared/upload-file/upload-file.module';
import { PhoneFormModule } from 'src/app/shared/phone-form/phone-form.module';



@NgModule({
  declarations: [RoleListComponent,RoleFormComponent],
  imports: [
    SharedModule,
    FormLayoutModule,
    LayoutModule,
    UploadFileModule,
    PhoneFormModule,
    RouterModule.forChild([
      { path: '', component: RoleListComponent },
      { path: '**', pathMatch: 'full', redirectTo: "managementRoles" }
    ]),
  ]
})
export class RolesModule { }
