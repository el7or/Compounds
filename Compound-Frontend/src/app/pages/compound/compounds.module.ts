import { CompoundsService } from '../../services/api/admin/compounds.service';
import { CompoundComponent } from './compound/compound.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SharedModule } from 'src/app/shared/shared.module';
import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { FormCompoundComponent } from './form-compound/form-compound.component';
import { LayoutModule } from 'src/app/shared/layout/layout.module';
import { UploadFileModule } from 'src/app/shared/upload-file/upload-file.module';
import { GoogleMapsModule } from '@angular/google-maps';
import { HttpClientModule, HttpClientJsonpModule } from '@angular/common/http';
import { CurrentUserService } from 'src/app/services/current-user.serives';
import { PhoneFormModule } from 'src/app/shared/phone-form/phone-form.module';

@NgModule({
  declarations: [CompoundComponent, FormCompoundComponent],
  imports: [
    CommonModule,
    SharedModule,
    FormLayoutModule,
    LayoutModule,
    HttpClientJsonpModule,
    HttpClientModule,
    UploadFileModule,
    GoogleMapsModule,
    PhoneFormModule,
    RouterModule.forChild([
      { path: '', component: CompoundComponent },
      { path: 'form-compound', component: FormCompoundComponent },
    ]),
  ],
  exports: [CompoundComponent],
  providers: [CompoundsService, CurrentUserService]
})
export class CompoundsModule { }
