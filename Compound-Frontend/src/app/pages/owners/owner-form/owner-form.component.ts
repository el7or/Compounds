import { OwnerType, SubOwnersService } from './../../../services/api/admin/sub-owners.service';
import { Owner } from './../../../services/api/admin/compound-owners.service';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { CompoundOwnersService } from 'src/app/services/api/admin/compound-owners.service';
import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { Subscription } from 'rxjs';
import moment from 'moment';
import { UnitsService } from 'src/app/services/api/admin/units.service';
import { MatTableDataSource } from '@angular/material/table';
import { SubOwner } from 'src/app/services/api/admin/sub-owners.service';
import { HttpParams } from '@angular/common/http';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';
import { CurrentUserService } from './../../../services/current-user.serives';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';

@Component({
  selector: 'app-owner-form',
  templateUrl: './owner-form.component.html',
  styleUrls: ['./owner-form.component.scss']
})
export class OwnerFormComponent implements OnInit, OnDestroy {
  compoundId!: string;
  form: FormGroup = Object.create(null);
  loading: boolean = false;
  ownerDetails!: Owner;
  minDate = moment().subtract(80, 'year').toISOString();
  maxDate = moment().subtract(21, 'year').toISOString();

  units: FormArray = new FormArray([
    new FormGroup({
      unitId: new FormControl(null, [Validators.required]),
      filterText: new FormControl(null),
    })
  ])
  companyUnits: any[] = [];
  get getOwnerUnits(): FormArray {
    return this.form.get('unitsIds') as FormArray;
  }
  displayedColumns = [
    'name',
    'phone',
    'birthDate',
    'userType',
    'unit',
    'fromTo',
    'status',
    'actions',
  ];
  dataSource!: MatTableDataSource<SubOwner>;

  subs = new Subscription();

  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }


  constructor(private fb: FormBuilder,
    public rightDrawer: RightDrawer,
    private activatedRoute: ActivatedRoute,
    private ownersService: CompoundOwnersService,
    private subOwnersService: SubOwnersService,
    private unitsService: UnitsService,
    private loadingService: LoadingService,
    public currentUserService: CurrentUserService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    public languageService: LanguageService) {
    this.activatedRoute.paramMap.subscribe((params) => {
      var compoundId = params.get('compoundId');
      if (compoundId != null) {
        this.compoundId = compoundId;
      }
      this.ownerDetails = this.rightDrawer.parentData;
    });
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      compoundOwnerId: [this.ownerDetails.compoundOwnerId],
      ownerRegistrationId: [this.ownerDetails.ownerRegistrationId],
      name: [this.ownerDetails.name, Validators.compose([Validators.required])],
      email: [this.ownerDetails.email, Validators.compose([Validators.required])],
      phone: [this.ownerDetails.phone, Validators.compose([Validators.required])],
      whatsAppNum: [this.ownerDetails.whatsAppNum ? this.ownerDetails.whatsAppNum : this.ownerDetails.phone],
      address: [this.ownerDetails.address],
      birthDate: [this.ownerDetails.birthDate],
      gender: [this.ownerDetails.gender],
      isActive: [this.ownerDetails.isActive, Validators.compose([Validators.required])],
      unitsIds: new FormArray([])
    });
    this.units.removeAt(0);
    this.ownerDetails.units.forEach((unit: any) => {
      this.units.push(
        new FormGroup({
          unitId: new FormControl(unit.compoundUnitId),
          filterText: new FormControl(unit.name),
        })
      )
    });
    this.selectUnit();
    if (!this.ownerDetails.fromTree)
      this.populateSubOwners();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
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

  ownerFilterSubscribes() {
    this.units.controls.forEach(control => {
      control.get('filterText')?.valueChanges.subscribe(text => {
        this.unitsService.unitsFilter({ pageNumber: 1, pageSize: 5, text, compoundId: this.compoundId })
          .subscribe(({ result }) => {
            this.companyUnits = result.result
          })
      })
    })
  }

  selectUnit() {
    while (this.getOwnerUnits.controls.length) {
      this.getOwnerUnits.removeAt(this.getOwnerUnits.controls.length - 1);
    }
    this.units.controls.forEach(control => {
      this.getOwnerUnits.push(new FormControl(control.value.unitId))
    })
  }

  onAddNewUnit(companuUnit: any, unitControl: any, selectedunitId: string) {
    if (companuUnit.ownersCount) {
      const message = this.languageService.lang == 'en' ? 'This unit already added to another owner, Please choose another unit !' : 'هذه الوحدة مضافة إلى مالك آخر بالفعل، الرجاء اختيار وحدة أخرى !';
      this.snackBar.open(message, '!!', {
        duration: 5000,
        horizontalPosition: 'start',
        panelClass: ['bg-danger']
      })
    }
    else {
      unitControl.get('unitId')?.setValue(selectedunitId);
      this.selectUnit();
    }
  }

  onSubmit() {
    this.loading = true;
    this.loadingService.itIsLoading = true;
    let ownerObj = new Owner();
    Object.assign(ownerObj, this.form.value);
    if (this.form.value.birthDate) {
      ownerObj.birthDate = new Date(this.form.value.birthDate).toDateString();
    }

    this.subs.add(
      this.ownersService.putCompoundOwner(ownerObj).subscribe(({ ok, result, message }) => {
        if (ok) {
          message = this.languageService.lang == 'en' ? 'Updated successfully !' : 'تم تعديل البيانات بنجاح !';
          this.snackBar.open(message, 'OK', {
            duration: 3000,
            horizontalPosition: 'start',
            panelClass: ['bg-success']
          })
          if (this.ownerDetails.fromTree)
            this.rightDrawer.close();
        } else {
          this.snackBar.open(message, 'ERROR', {
            duration: 3000,
            horizontalPosition: 'start',
            panelClass: ['bg-danger']
          })
        }
        this.loadingService.itIsLoading = false;
        this.loading = false;
      })
    );
  }

  populateSubOwners() {
    this.subs.add(
      this.subOwnersService.getSubOwners(new HttpParams().set('mainRegistrationId', this.ownerDetails.ownerRegistrationId))
        .subscribe(({ ok, result, message }) => {
          if (ok) {
            result = result.map((subOwner: SubOwner) => {
              subOwner.userType = OwnerType[subOwner.userType];
              return subOwner;
            });
            this.dataSource = new MatTableDataSource(result);
          } else {
            this.snackBar.open(message, 'ERROR', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-danger']
            })
          }
          this.loadingService.itIsLoading = false;
        })
    );
  }

  onChangeStatus(subOwner: SubOwner) {
    this.loadingService.itIsLoading = true;
    subOwner.isActive = !subOwner.isActive;
    if (!subOwner.isActive) {
      this.subs.add(
        this.subOwnersService.deActiveSubOwner(subOwner.ownerRegistrationId).subscribe(({ ok, result, message }) => {
          if (ok) {
            message = this.languageService.lang == 'en' ? 'Deactivated successfully !' : 'تم التعطيل بنجاح !';
            this.snackBar.open(message, 'OK', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-success']
            })
          } else {
            this.snackBar.open(message, 'ERROR', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-danger']
            })
          }
          this.loadingService.itIsLoading = false;
        })
      );
    }
    else {
      this.subs.add(
        this.subOwnersService.activeSubOwner(subOwner.ownerRegistrationId).subscribe(({ ok, result, message }) => {
          if (ok) {
            message = this.languageService.lang == 'en' ? 'Activated Successfully !' : 'تم التفعيل بنجاح !';
            this.snackBar.open(message, 'OK', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-success']
            })
          } else {
            this.snackBar.open(message, 'ERROR', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-danger']
            })
          }
          this.loadingService.itIsLoading = false;
        })
      );
    }
  }

  onDelete(event: any, subOwner: SubOwner) {
    let targetAttr = event.target.getBoundingClientRect();

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    if (this.languageService.lang == 'en') {
      dialogConfig.data = {
        massage: "Are you sure to delete this Sub owner?"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width - 80 + "px"
      };
    } else {
      dialogConfig.data = {
        massage: "هل أنت متأكد من حذف هذا المالك الفرعي"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width + 80 + "px"
      };
    }

    dialogConfig.backdropClass = 'dialog-bg-trans';
    const dialogRef = this.dialog.open(DialogLayoutComponent, dialogConfig
    );

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadingService.itIsLoading = true;
        this.subs.add(
          this.subOwnersService.DeleteSubOwner(subOwner.ownerRegistrationId).subscribe(({ ok, result, message }) => {
            if (ok) {
              this.dataSource.data.splice(this.dataSource.data.indexOf(subOwner), 1);
              this.dataSource._updateChangeSubscription();
              message = this.languageService.lang == 'en' ? 'Deleted Successfully !' : 'تم الحذف بنجاح !';
              this.snackBar.open(message, 'OK', {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-success']
              })
            } else {
              this.snackBar.open(message, 'ERROR', {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-danger']
              })
            }
            this.loadingService.itIsLoading = false;
          })
        );
      }
    });
  }
}
