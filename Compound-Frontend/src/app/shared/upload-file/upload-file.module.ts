import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';
import { MatRippleModule } from '@angular/material/core';
import { UploadFileComponent } from './upload-file.component';
import { SharedModule } from '../shared.module';


@NgModule({
  declarations: [UploadFileComponent],
  imports: [
    CommonModule,
    MatIconModule,
    MatRippleModule,
    SharedModule
  ],
  exports: [UploadFileComponent]
})
export class UploadFileModule { }
