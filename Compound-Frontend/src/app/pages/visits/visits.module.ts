import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { LayoutModule } from 'src/app/shared/layout/layout.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { UploadFileModule } from 'src/app/shared/upload-file/upload-file.module';
import { VisitsListComponent } from './visits-list/visits-list.component';
import { VisitViewComponent } from './visit-view/visit-view.component';
import { MatCarouselModule } from '@ngmodule/material-carousel';


@NgModule({
  declarations: [VisitsListComponent, VisitViewComponent],
  imports: [
    SharedModule,
    FormLayoutModule,
    LayoutModule,
    UploadFileModule,
    MatCarouselModule.forRoot(),
    RouterModule.forChild([
      { path: '', component: VisitsListComponent },
      { path: '**', pathMatch: 'full', redirectTo: "visits" }
    ])
  ]
})
export class VisitsModule { }
