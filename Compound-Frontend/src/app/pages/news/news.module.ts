import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MatCarouselModule } from '@ngmodule/material-carousel';

import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { LayoutModule } from 'src/app/shared/layout/layout.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { UploadFileModule } from 'src/app/shared/upload-file/upload-file.module';
import { NewsFormComponent } from './news-form/news-form.component';
import { NewsListComponent } from './news-list/news-list.component';



@NgModule({
  declarations: [NewsListComponent, NewsFormComponent],
  imports: [
    SharedModule,
    FormLayoutModule,
    LayoutModule,
    UploadFileModule,
    MatCarouselModule.forRoot(),
    RouterModule.forChild([
      { path: '', component: NewsListComponent },
      { path: '**', pathMatch: 'full', redirectTo: "news" }
    ])
  ]
})
export class NewsModule { }
