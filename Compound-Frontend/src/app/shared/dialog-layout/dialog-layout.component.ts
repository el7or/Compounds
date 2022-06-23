import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CompoundsService } from 'src/app/services/api/admin/compounds.service';

@Component({
  selector: 'app-dialog-layout',
  templateUrl: './dialog-layout.component.html',
  styleUrls: ['./dialog-layout.component.scss']
})
export class DialogLayoutComponent implements OnInit {
  massage: string = '';

  constructor(
    public dialogRef: MatDialogRef<DialogLayoutComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit(): void {
    this.massage = this.data.massage;
  }
}
