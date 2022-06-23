import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReplyLayoutComponent } from './reply-layout.component';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@NgModule({
  declarations: [ReplyLayoutComponent],
  imports: [
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule
  ],
  exports:[
    ReplyLayoutComponent
  ]
})
export class ReplyLayoutModule { }
