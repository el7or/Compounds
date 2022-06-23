import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CompoundGroupsService } from 'src/app/services/api/admin/compound-groups.service';
import { RightDrawer } from 'src/app/services/right-drawer.serives';

@Component({
  selector: 'app-form-group',
  templateUrl: './form-group.component.html',
  styleUrls: ['./form-group.component.scss']
})
export class FormGroupComponent implements OnInit {
  loading: boolean = false;
  caption :string='unit.formGroup.add';
  form: FormGroup = new FormGroup({
    compoundGroupId: new FormControl(null),
    nameAr: new FormControl(null,[Validators.required,Validators.minLength(3)]),
    nameEn: new FormControl(null, [Validators.required,Validators.minLength(3)]),
    compoundId: new FormControl(this.rightDrawer.parentData.compoundId ),
    parentGroupId: new FormControl(this.rightDrawer.parentData.compoundGroupId ),
  });

  constructor(
    public rightDrawer: RightDrawer,
    private compoundGroupsService: CompoundGroupsService,
    private snackBar: MatSnackBar,
  ) { }

  ngOnInit(): void {
    if (this.rightDrawer.parentData.forEdit) {
      this.form.patchValue(this.rightDrawer.parentData);
      this.caption='unit.formGroup.edit';
    }
  }

  submit() {
    this.loading = true;
    let service: any;
    if (this.form.value.compoundGroupId) {
      service = this.compoundGroupsService.putCompoundGroups(this.form.value);
    } else {
      service = this.compoundGroupsService.postCompoundGroups(this.form.value)
    }
    service.subscribe(({ ok, message, result }: any) => {
      this.loading = false;
      if (ok) {
        this.rightDrawer.close(this.form.value);
      } else {
        this.snackBar.open(message, 'OK', {
          duration: 3000,
          horizontalPosition: 'start',
          panelClass: ['bg-danger']
        });
      }
    })
  }
}
