import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { LayoutModule } from 'src/app/shared/layout/layout.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { OwnerListComponent } from './owner-list/owner-list.component';
import { OwnerFormComponent } from './owner-form/owner-form.component';
import { PhoneFormModule } from 'src/app/shared/phone-form/phone-form.module';


@NgModule({
  declarations: [OwnerListComponent, OwnerFormComponent, ],
  imports: [
    SharedModule,
    FormLayoutModule,
    LayoutModule,
    PhoneFormModule,
    RouterModule.forChild([
      { path: '', component: OwnerListComponent },
      { path: '**', pathMatch: 'full', redirectTo: "owner" }
    ]),
  ]
})
export class OwnersModule { }
