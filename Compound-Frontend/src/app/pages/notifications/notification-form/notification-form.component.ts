import { IGroup } from './../../../services/api/admin/compound-groups.service';
import { Notification } from './../../../services/api/admin/notifications.service';
import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { NotificationsService } from 'src/app/services/api/admin/notifications.service';
import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { CompoundsService } from 'src/app/services/api/admin/compounds.service';
import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { IUnit } from 'src/app/services/api/admin/units.service';
import { map, startWith } from 'rxjs/operators';
import { CurrentUserService } from './../../../services/current-user.serives';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';

@Component({
  selector: 'app-notification-form',
  templateUrl: './notification-form.component.html',
  styleUrls: ['./notification-form.component.scss']
})
export class NotificationFormComponent implements OnInit, OnDestroy {
  compoundId!: string;
  notificationId?: string | null;
  form: FormGroup = Object.create(null);
  loading: boolean = false;
  today = new Date().toISOString();

  separatorKeysCodes: number[] = [ENTER, COMMA];

  // all props for select groups control
  compoundGroups: IGroup[] = [];
  filteredGroups!: Observable<string[]>;
  allGroups!: string[];
  selectedGroups!: string[];
  @ViewChild('groupInput') groupInput!: ElementRef<HTMLInputElement>;
  private _filterGroup(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.allGroups.filter(group => group.toLowerCase().includes(filterValue));
  }

  // all props for select units control
  compoundUnits: IUnit[] = [];
  filteredUnits!: Observable<string[]>;
  allUnits!: string[];
  selectedUnits!: string[];
  @ViewChild('unitInput') unitInput!: ElementRef<HTMLInputElement>;
  private _filterUnit(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.allUnits.filter(unit => unit.toLowerCase().includes(filterValue));
  }

  subs = new Subscription();

  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }

  constructor(private fb: FormBuilder,
    public rightDrawer: RightDrawer,
    private activatedRoute: ActivatedRoute,
    private notificationService: NotificationsService,
    private compoundsService: CompoundsService,
    public currentUserService: CurrentUserService,
    private loadingService: LoadingService,
    private snackBar: MatSnackBar,
    public languageService: LanguageService) {
    this.activatedRoute.paramMap.subscribe((params) => {
      var compoundId = params.get('compoundId');
      if (compoundId != null) {
        this.compoundId = compoundId;
      }
      this.notificationId = this.rightDrawer.parentData;
    });
  }

  ngOnInit(): void {
    this.initForm();

    this.compoundsService.getAllGroupsByCompoundId(this.compoundId).subscribe(({ ok, result, message }) => {
      if (ok) {
        // for select groups control
        this.populateGroups(result);

        // for select units control
        this.populateGroupUnits(result);

        //if edit notification
        if (this.notificationId) this.patchForm();
      } else {
        this.snackBar.open(message, 'ERROR', {
          duration: 3000,
          horizontalPosition: 'start',
          panelClass: ['bg-danger']
        });
      }
    })

  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  initForm() {
    this.form = this.fb.group({
      compoundNotificationId: [null],
      compoundId: [this.compoundId, Validators.compose([Validators.required])],
      englishTitle: [null, Validators.compose([Validators.required])],
      arabicTitle: [null, Validators.compose([Validators.required])],
      englishMessage: [null, Validators.compose([Validators.required])],
      arabicMessage: [null, Validators.compose([Validators.required])],
      toGroups: [null],
      toUnits: [null],
      isOwnerOnly: [null],
      images: [null, Validators.compose([Validators.required])],
    });
  }

  populateGroups(compoundGroups: IGroup[]) {
    this.compoundGroups = compoundGroups.filter(g => g.compoundUnits?.length);
    this.allGroups = this.compoundGroups.map(group => { return this.languageService.lang == 'en' ? group.nameEn : group.nameAr });
    this.filteredGroups = this.form.controls['toGroups'].valueChanges.pipe(
      startWith(null),
      map((group: string | null) => group ? this._filterGroup(group) : this.allGroups.slice()));
  }

  populateGroupUnits(compoundGroups: IGroup[]) {
    compoundGroups.forEach(group => {
      if (group.compoundUnits?.length) this.compoundUnits.push(...group.compoundUnits)
    });
    this.allUnits = this.compoundUnits.map(unit => { return unit.name });
    this.filteredUnits = this.form.controls['toUnits'].valueChanges.pipe(
      startWith(null),
      map((unit: string | null) => unit ? this._filterUnit(unit) : this.allUnits.slice()));
  }

  patchForm() {
    this.subs.add(
      this.notificationService.getNotification(this.notificationId!).subscribe(({ ok, result, message }) => {
        if (ok) {
          this.form.patchValue({
            compoundNotificationId: this.notificationId,
            compoundId: this.compoundId,
            englishTitle: result.englishTitle,
            arabicTitle: result.arabicTitle,
            englishMessage: result.englishMessage,
            arabicMessage: result.arabicMessage,
            // toGroups: result.toGroupsIds,
            // toUnits: result.toUnitsIds,
            isOwnerOnly: result.isOwnerOnly,
            images: result.images
          });
          result.toGroupsIds.forEach((groupId: string) => {
            const group = this.compoundGroups.find(g => g.compoundGroupId == groupId);
            this.selectGroup(this.languageService.lang == 'en' ? group!.nameEn : group!.nameAr, false)
          });
          result.toUnitsIds.forEach((unitId: string) => {
            const unit = this.compoundUnits.find(g => g.compoundUnitId == unitId);
            this.selectUnit(unit!.name);
          });
        } else {
          this.snackBar.open(message, 'ERROR', {
            duration: 3000,
            horizontalPosition: 'start',
            panelClass: ['bg-danger']
          });
        }
      })
    );
  }

  selectGroup(value: string, withUnits?:boolean): void {
    this.selectedGroups ? this.selectedGroups!.push(value) : this.selectedGroups = [value];
    const index = this.allGroups!.indexOf(value);
    if (index >= 0) {
      this.allGroups!.splice(index, 1);
    }
    this.groupInput.nativeElement.value = '';
    this.form.controls['toGroups'].setValue(null);

    // select units for selected group
    if(withUnits){
    const selectedGroup = this.compoundGroups.filter(group => group.nameEn == value || group.nameAr == value);
    selectedGroup[0].compoundUnits!.forEach(unit => this.selectUnit(unit.name));
    }
  }

  removeGroup(value: string): void {
    const index = this.selectedGroups!.indexOf(value);
    if (index >= 0) {
      this.selectedGroups!.splice(index, 1);
    }
    this.allGroups ? this.allGroups!.push(value) : this.allGroups = [value];

    // remove units for removed group
    const removedGroup = this.compoundGroups.filter(group => group.nameEn == value || group.nameAr == value);
    removedGroup[0].compoundUnits!.forEach(unit => this.removeUnit(unit.name));
  }

  selectUnit(value: string): void {
    if (!this.selectedUnits?.includes(value))
      this.selectedUnits ? this.selectedUnits!.push(value) : this.selectedUnits = [value];
    const index = this.allUnits!.indexOf(value);
    if (index >= 0) {
      this.allUnits!.splice(index, 1);
    }
    this.unitInput.nativeElement.value = '';
    this.form.controls['toUnits'].setValue(null);
  }

  removeUnit(value: string): void {
    const index = this.selectedUnits!.indexOf(value);
    if (index >= 0) {
      this.selectedUnits!.splice(index, 1);
    }
    if (!this.allUnits?.includes(value))
    this.allUnits ? this.allUnits!.push(value) : this.allUnits = [value];
  }

  onSubmit() {
    this.loading = true;
    this.loadingService.itIsLoading = true;
    let notificationObj = new Notification();
    Object.assign(notificationObj, this.form.value);
    if (this.selectedUnits) {
      notificationObj.toUnitsIds = this.selectedUnits!.map(unit => {
        return this.compoundUnits.find(u => u.name === unit)?.compoundUnitId!;
      });
    } else {
      notificationObj.toUnitsIds = this.compoundUnits!.map(unit => {
        return unit.compoundUnitId;
      });
    }

    // add notification
    if (!this.notificationId) {
      this.subs.add(
        this.notificationService.postNotification(notificationObj).subscribe(({ ok, result, message }) => {
          if (ok) {
            message = this.languageService.lang == 'en' ? 'Sent successfully !' : 'تم إرسال الإشعار بنجاح !';
            this.snackBar.open(message, 'OK', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-success']
            })
            this.rightDrawer.close();
          }
          else {
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
    // edit notification
    else {
      this.subs.add(
        this.notificationService.putNotification(notificationObj).subscribe(({ ok, result, message }) => {
          if (ok) {
            message = this.languageService.lang == 'en' ? 'Updated successfully !' : 'تم تعديل الإشعار بنجاح !';
            this.snackBar.open(message, 'OK', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-success']
            })
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
  }
}
