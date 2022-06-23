import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogLayoutComponent } from './dialog-layout.component';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';
import { SharedModule } from '../shared.module';



@NgModule({
  declarations: [DialogLayoutComponent],
  imports: [
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule,
    SharedModule
  ],
  exports:[
    DialogLayoutComponent
  ]
})
export class DialogLayoutModule { }
