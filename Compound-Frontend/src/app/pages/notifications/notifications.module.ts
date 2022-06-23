import { NgModule } from '@angular/core';
import { NotificationListComponent } from './notification-list/notification-list.component';
import { NotificationFormComponent } from './notification-form/notification-form.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { LayoutModule } from 'src/app/shared/layout/layout.module';
import { UploadFileModule } from 'src/app/shared/upload-file/upload-file.module';
import { MatCarouselModule } from '@ngmodule/material-carousel';
import { RouterModule } from '@angular/router';



@NgModule({
  declarations: [NotificationListComponent, NotificationFormComponent],
  imports: [
    SharedModule,
    FormLayoutModule,
    LayoutModule,
    UploadFileModule,
    MatCarouselModule.forRoot(),
    RouterModule.forChild([
      { path: '', component: NotificationListComponent },
      { path: '**', pathMatch: 'full', redirectTo: "news" }
    ])
  ]
})
export class NotificationsModule { }
