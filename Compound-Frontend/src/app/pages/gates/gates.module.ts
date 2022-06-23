import { LayoutModule } from './../../shared/layout/layout.module';
import { FormGateComponent } from './form-gate/form-gate.component';
import { GateComponent } from './gate/gate.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { GatesService } from 'src/app/services/api/admin/gates.service';



@NgModule({
  declarations: [GateComponent,FormGateComponent],
  imports: [
    SharedModule,
    FormLayoutModule,
    LayoutModule,
    RouterModule.forChild([
      { path: '', component: GateComponent },
      { path: 'form-gate', component: FormGateComponent },
      { path: '**', pathMatch: 'full', redirectTo: "gate" }
    ]),
  ],
  providers: [GatesService]
})
export class GatesModule { }
