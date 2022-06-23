import { CompoundOwnersService } from './../../../services/api/admin/compound-owners.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormArray } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LanguageService } from 'src/app/services/language.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';

@Component({
  selector: 'app-form-owner',
  templateUrl: './form-owner.component.html',
  styleUrls: ['./form-owner.component.scss']
})
export class FormOwnerComponent implements OnInit {
  loading: boolean = false;
  ownerBasicInfoForm: FormGroup = new FormGroup({
    name: new FormControl(null, [Validators.required, Validators.minLength(3)]),
    address: new FormControl(null, [Validators.required, Validators.minLength(3)]),
  });
  ownerContactForm: FormGroup = new FormGroup({
    phone: new FormControl(null, [Validators.required]),
    email: new FormControl(null, [Validators.required, Validators.email]),
    ownerRegistrationId: new FormControl(null),
    subOwnersCount: new FormControl(0),
    compoundOwnerId: new FormControl(null),
    unitsIds: new FormControl(null),
  });

  constructor(
    public rightDrawer: RightDrawer,
    private snackBar: MatSnackBar,
    public languageService: LanguageService,
    private compoundOwnersService: CompoundOwnersService
  ) { }

  ngOnInit(): void {
    if (this.rightDrawer.parentData) {
      if (this.rightDrawer.parentData.forEdit) {
        this.ownerContactForm.patchValue(this.rightDrawer.parentData);
      } else {
        const parentUnit: string[] = [this.rightDrawer.parentData.compoundUnitId]
        this.ownerContactForm.patchValue({
          unitsIds: parentUnit,
        });
      }
    }
  }

  submit() {
    this.loading = true;
    let service: any;
    let fromValue = {
      ...this.ownerBasicInfoForm.value,
      ...this.ownerContactForm.value
    }
    if (fromValue.unitsIds.length) {
      service = this.compoundOwnersService.postCompoundOwner(fromValue);
    } else {
      service = this.compoundOwnersService.putCompoundOwner(fromValue)
    }
    service.subscribe(({ ok, message, result }: any) => {
      this.loading = false;
      if (ok) {
        this.rightDrawer.close(fromValue);
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
