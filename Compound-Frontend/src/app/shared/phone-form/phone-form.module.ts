import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PhoneFormComponent } from './phone-form.component';
import { PhoneFormService } from './phone-form.service';
import { SharedModule } from '../shared.module';
import { MatAutocompleteModule } from '@angular/material/autocomplete';



@NgModule({
  declarations: [PhoneFormComponent],
  imports: [
    CommonModule,
    SharedModule,
    MatAutocompleteModule
  ],
  exports: [PhoneFormComponent],
  providers: [PhoneFormService]
})
export class PhoneFormModule { }
