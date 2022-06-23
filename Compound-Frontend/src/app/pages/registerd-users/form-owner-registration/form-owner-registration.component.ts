import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators, FormArray } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LanguageService } from 'src/app/services/language.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import * as moment from "moment";
import { CompoundOwnersService } from 'src/app/services/api/admin/compound-owners.service';
import { UnitRequestsService } from 'src/app/services/api/admin/unit-requests.service';
import { UnitsService } from 'src/app/services/api/admin/units.service';
import { MatOptionSelectionChange } from '@angular/material/core';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { CurrentUserService } from 'src/app/services/current-user.serives';
import { ActivatedRoute } from '@angular/router';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';
@Component({
  selector: 'app-form-owner-registration',
  templateUrl: './form-owner-registration.component.html',
  styleUrls: ['./form-owner-registration.component.scss']
})
export class FormOwnerRegistrationComponent implements OnInit {
  compoundId!: string;
  maxDate = moment().subtract(21, 'year').toISOString();
  minDate = moment().subtract(80, 'year').toISOString();
  loading: boolean = false;
  private mobileReg = '^((\\+91-?)|0)?[0-9]{11}$';
  ownerForm: FormGroup = new FormGroup({
    name: new FormControl(null, [Validators.required, Validators.minLength(3)]),
    email: new FormControl(null, [Validators.required, Validators.email]),
    phone: new FormControl(null, [Validators.required]),
    // whatsAppNum: new FormControl(null, [Validators.required]),
    // address: new FormControl(null, [Validators.required, Validators.minLength(3)]),
    // birthDate: new FormControl(null, [Validators.required]),

    gender: new FormControl(1),
    // image: new FormGroup({
    //   sizeInBytes: new FormControl(0),
    //   fileName: new FormControl(null),
    //   path: new FormControl(null),
    //   fileBase64: new FormControl(null)
    // }),

    ownerRegistrationId: new FormControl(null),
    compoundOwnerId: new FormControl(null),
    units: new FormArray([])
  });
  get getOwnerUnits(): FormArray {
    return this.ownerForm.get('units') as FormArray;
  }
  units: FormArray = new FormArray([
    new FormGroup({
      unitId: new FormControl(null, [Validators.required]),
      filterText: new FormControl(null),
    })
  ])
  registeredUsersUnits: any[] = [];
  ownerRegistrationInfo: any;
  companyUnits: any[] = [];

  allOwners: any[] = [];
  filteredOwnersByName!: Observable<any[]>;
  filteredOwnersByEmail!: Observable<any[]>;
  filteredOwnersByPhone!: Observable<any[]>;
  private _filterByName(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.allOwners.filter(owner => owner.name.toLowerCase().includes(filterValue));
  }
  private _filterByEmail(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.allOwners.filter(owner => owner.email.toLowerCase().includes(filterValue));
  }
  private _filterByPhone(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.allOwners.filter(owner => owner.phone.toLowerCase().includes(filterValue));
  }
  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }

  constructor(
    public rightDrawer: RightDrawer,
    private snackBar: MatSnackBar,
    public languageService: LanguageService,
    private activatedRoute: ActivatedRoute,
    private compoundOwnersService: CompoundOwnersService,
    private unitRequestsService: UnitRequestsService,
    private unitsService: UnitsService,
    public currentUserService: CurrentUserService
  ) {
    this.activatedRoute.paramMap.subscribe((params) => {
      var compoundId = params.get('compoundId');
      if (compoundId != null) {
        this.compoundId = compoundId;
      }
    });
   }
  ngOnInit(): void {
    if (this.rightDrawer.parentData) {
      this.ownerRegistrationInfo = this.rightDrawer.parentData.ownerRegistration.info;
      this.registeredUsersUnits = this.rightDrawer.parentData.ownerRegistration.units;
      if (this.rightDrawer.parentData.owner) {
        this.rightDrawer.parentData.owner.info.ownerRegistrationId = this.rightDrawer.parentData.ownerRegistration.info.ownerRegistrationId;
        if (this.rightDrawer.parentData.owner.units.length) {
          this.rightDrawer.parentData.owner.info.units = this.rightDrawer.parentData.owner.units.map((i: any) => i.compoundUnitId);
          this.units.removeAt(0);
          this.rightDrawer.parentData.owner.units.forEach((unit: any) => {
            this.units.push(
              new FormGroup({
                unitId: new FormControl(unit.compoundUnitId),
                filterText: new FormControl(unit.name),
              })
            )
          });
        }
        this.ownerForm.patchValue(this.rightDrawer.parentData.owner.info);
        this.selectUnit();
      } else {
        if (this.rightDrawer.parentData.ownerRegistration.units.length) {
          this.rightDrawer.parentData.ownerRegistration.info.units = this.rightDrawer.parentData.ownerRegistration.units.map((i: any) => i.compoundUnitId);
          this.units.removeAt(0);
          this.rightDrawer.parentData.ownerRegistration.units.forEach((unit: any) => {
            this.units.push(
              new FormGroup({
                unitId: new FormControl(unit.compoundUnitId),
                filterText: new FormControl(unit.name),
              })
            )
          });
        }
        this.compoundOwnersService.getCompoundOwners(new HttpParams().set('companyId', this.currentUserService.currentUser.companyId)).subscribe(({ result, ok, message }) => {
          if (ok) {
            this.loading = false;
            this.allOwners = result;
            this.filteredOwnersByName = this.ownerForm.controls['name'].valueChanges.pipe(
              startWith(''), map(value => this._filterByName(value)));
            this.filteredOwnersByEmail = this.ownerForm.controls['email'].valueChanges.pipe(
              startWith(''), map(value => this._filterByEmail(value)));
            this.filteredOwnersByPhone = this.ownerForm.controls['phone'].valueChanges.pipe(
              startWith(''), map(value => this._filterByPhone(value)));
          } else {
            this.snackBar.open(message, 'ERROR', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-danger']
            })
          }
        });
        //this.ownerForm.patchValue({ ownerRegistrationId: this.rightDrawer.parentData.ownerRegistration.info.ownerRegistrationId });
        this.selectUnit();
      }
    }
    this.ownerFilterSubscribes();
  }

  selectOwner(event: MatOptionSelectionChange) {
    const selectedUser = this.allOwners.find(i => i.compoundOwnerId == event.source.id);
    this.ownerForm.patchValue(selectedUser);
    this.allOwners.length = 0
    this.selectUnit();
  }

  selectUnit() {
    while (this.getOwnerUnits.controls.length) {
      this.getOwnerUnits.removeAt(this.getOwnerUnits.controls.length - 1);
    }
    this.units.controls.forEach(control => {
      this.getOwnerUnits.push(new FormControl(control.value.unitId))
    })
  }

  ownerFilterSubscribes() {
    this.units.controls.forEach(control => {
      control.get('filterText')?.valueChanges.subscribe(text => {
        this.unitsService.unitsFilter({ pageNumber: 1, pageSize: 5, text, compoundId: this.compoundId }).subscribe(({ result }) => this.companyUnits = result.result)
      })
    })
  }

  addUnite() {
    this.units.push(
      new FormGroup({
        unitId: new FormControl(null, [Validators.required]),
        filterText: new FormControl(null),
      })
    );
    this.ownerFilterSubscribes();
  }

  submit() {
    this.loading = true;
    // this.unitRequestsService.postApprove({ compoundOwnerId: this.ownerForm.value.compoundOwnerId, ownerRegistrationId: this.ownerForm.value.ownerRegistrationId }).subscribe(({ result, ok, message }) => {
    //   if (ok) {
    //     this.loading = false;
    //     this.rightDrawer.close(result);
    //     message = this.languageService.lang == 'en' ? 'Confirmed Successfully !' : 'تم التأكيد بنجاح !';
    //     this.snackBar.open(message, 'OK', {
    //       duration: 3000,
    //       horizontalPosition: 'start',
    //       panelClass: ['bg-success']
    //     })
    //   } else {
    //     this.snackBar.open(message, 'ERROR', {
    //       duration: 3000,
    //       horizontalPosition: 'start',
    //       panelClass: ['bg-danger']
    //     })
    //   }
    // })




    let service: any;
    if (this.ownerForm.value.compoundOwnerId) {
      service = this.compoundOwnersService.putCompoundOwner(this.ownerForm.value);
    } else {
      service = this.compoundOwnersService.postCompoundOwner(this.ownerForm.value);
    }
    service.subscribe(({ ok, message, result }: any) => {
      if (ok) {
        this.unitRequestsService.postApprove({ compoundOwnerId: result.compoundOwnerId, ownerRegistrationId: this.ownerRegistrationInfo.ownerRegistrationId }).subscribe(({ result, ok }) => {
          this.rightDrawer.close(result);
          this.loading = false;
          message = this.languageService.lang == 'en' ? 'Confirmed Successfully !' : 'تم التأكيد بنجاح !';
          this.snackBar.open(message, 'OK', {
            duration: 3000,
            horizontalPosition: 'start',
            panelClass: ['bg-success']
          })
        })
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
