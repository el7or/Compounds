import { ServiceComponent } from './service/service.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { LayoutModule } from 'src/app/shared/layout/layout.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { FormServiceComponent } from './form-service/form-service.component';
import { ServiceRequestComponent } from './service-request/service-request.component'
import { MatCarouselModule } from '@ngmodule/material-carousel';

@NgModule({
  declarations: [ServiceComponent, FormServiceComponent, ServiceRequestComponent],
  imports: [
    SharedModule,
    FormLayoutModule,
    LayoutModule,
    MatCarouselModule.forRoot(),
    RouterModule.forChild([
      { path: '', component: ServiceComponent },
      { path: 'form-service', component: FormServiceComponent },
      { path: '**', pathMatch: 'full', redirectTo: "service" }
    ]),
  ]

})
export class ServicesModule { }
