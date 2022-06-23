import { NgModule } from '@angular/core';
import { AdsListComponent } from './ads-list/ads-list.component';
import { AdsFormComponent } from './ads-form/ads-form.component';
import { RouterModule } from '@angular/router';
import { MatCarouselModule } from '@ngmodule/material-carousel';
import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { LayoutModule } from 'src/app/shared/layout/layout.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { UploadFileModule } from 'src/app/shared/upload-file/upload-file.module';



@NgModule({
  declarations: [AdsListComponent, AdsFormComponent],
  imports: [
    SharedModule,
    FormLayoutModule,
    LayoutModule,
    UploadFileModule,
    MatCarouselModule.forRoot(),
    RouterModule.forChild([
      { path: '', component: AdsListComponent },
      { path: '**', pathMatch: 'full', redirectTo: "ads" }
    ])
  ]
})
export class AdsModule { }
