import { DialogLayoutModule } from './../../shared/dialog-layout/dialog-layout.module';
import { FormUnitComponent } from './form-unit/form-unit.component';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { LayoutModule } from 'src/app/shared/layout/layout.module';
import { FormGroupComponent } from './form-group/form-group.component';
import { FormOwnerComponent } from './form-owner/form-owner.component';
import { CompoundTreeComponent } from './compound-tree/compound-tree.component';
import { HttpClientJsonpModule, HttpClientModule } from '@angular/common/http';
import { UploadFileModule } from 'src/app/shared/upload-file/upload-file.module';
import { CommonModule } from '@angular/common';
import { PhoneFormModule } from 'src/app/shared/phone-form/phone-form.module';


@NgModule({
  declarations: [CompoundTreeComponent, FormUnitComponent, FormGroupComponent, FormOwnerComponent],
  imports: [
    CommonModule,
    DialogLayoutModule,
    SharedModule,
    FormLayoutModule,
    LayoutModule,
    HttpClientJsonpModule,
    HttpClientModule,
    UploadFileModule,
    PhoneFormModule,
    RouterModule.forChild([
      { path: '', component: CompoundTreeComponent },
      { path: ' form-unit', component: FormUnitComponent },
      { path: ' form-group', component: FormGroupComponent },
      { path: '**', pathMatch: 'full', redirectTo: "unit" }
    ]),
  ]
})
export class UnitsModule { }
