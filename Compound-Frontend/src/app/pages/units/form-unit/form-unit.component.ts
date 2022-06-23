import { OnInit } from '@angular/core';
import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UnitTypesService } from 'src/app/services/api/admin/unit-types.service';
import { UnitsService } from 'src/app/services/api/admin/units.service';
import { LanguageService } from 'src/app/services/language.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';

@Component({
  selector: 'app-form-unit',
  templateUrl: './form-unit.component.html',
  styleUrls: ['./form-unit.component.scss']
})
export class FormUnitComponent implements OnInit {
  loading: boolean = false;
  caption :string="unit.formUnit.add";
  unitTypes: any[] = [];
  form: FormGroup = new FormGroup({
    compoundUnitId: new FormControl(this.rightDrawer.parentData.compoundUnitId),
    name: new FormControl(null,[Validators.required,Validators.minLength(3)]),
    compoundGroupId: new FormControl(this.rightDrawer.parentData.compoundGroupId, [Validators.required]),
    compoundUnitTypeId: new FormControl(null, [Validators.required]),
  });

  constructor(
    public rightDrawer: RightDrawer,
    private snackBar: MatSnackBar,
    private unitsService: UnitsService,
    private unitTypesService: UnitTypesService,
    public languageService: LanguageService,
  ) { }

  ngOnInit(): void {
    
    if (this.rightDrawer.parentData.forEdit) {
      this.caption="unit.formUnit.edit";
      this.form.patchValue(this.rightDrawer.parentData);
    //   this.form.addControl('compoundUnitId',new FormControl(this.rightDrawer.parentData.compoundUnitId, [Validators.required]))
    }
    this.unitTypesService.getUnitType().subscribe(({ result }) => this.unitTypes = result);
  }

  submit() {
    this.loading = true;
    let service: any;
    if (this.form.value.compoundUnitId) {
      service = this.unitsService.putUnits(this.form.value);
    } else {
      service = this.unitsService.postUnits(this.form.value)
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
