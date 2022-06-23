import { SystemPage, SystemPageAction, SystemPagesService } from './../../../services/api/admin/system-pages.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatStepper } from '@angular/material/stepper';
import { Observable, Subscription } from 'rxjs';
import { RolesService } from 'src/app/services/api/admin/roles.service';
import { CurrentUserService } from 'src/app/services/current-user.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { CompanyRoleAction, CompanyRoleActionsService, UpdatePagesActionsInRoles } from 'src/app/services/api/admin/company-role-actions.service';
import { CompanyUser, CompanyUsersService, UsersFilter } from 'src/app/services/api/admin/company-users.service';
import { CompanyUserRole, CompanyUserRolesService, UpdateUserRoles } from 'src/app/services/api/admin/company-user-roles.service';
import Swal from 'sweetalert2';
import { TranslateService } from '@ngx-translate/core';
import { map, startWith } from 'rxjs/operators';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';


@Component({
  selector: 'app-role-form',
  templateUrl: './role-form.component.html',
  styleUrls: ['./role-form.component.scss']
})
export class RoleFormComponent implements OnInit, OnDestroy {
  loading: boolean = false;
  roleId?: string;
  allPagesActions!: SystemPage[];
  usersFilter: UsersFilter = new UsersFilter();
  currentLang!: string;

  companyRoleForm: FormGroup = Object.create(null);
  compoundContactForm: FormGroup = new FormGroup({
    phone: new FormControl(null, [Validators.required]),
    mobile: new FormControl(null, [Validators.required]),
    email: new FormControl(null, [Validators.email])
  });

  allCompanyUsers!: CompanyUser[];
  companyUsers!: CompanyUser[];
  usersControl = new FormControl();
  filteredOptions!: Observable<CompanyUser[]>;
  private _filter(name: string): CompanyUser[] {
    const filterValue = name.toLowerCase();
    return this.allCompanyUsers.filter(user =>
      this.currentLang == 'en' ? user.nameEn?.toLowerCase().includes(filterValue) : user.nameAr?.toLowerCase().includes(filterValue)
    );
  }
  displayFn(user: CompanyUser): string {
    if (user) {
      return this.currentLang == 'en' ? user.nameEn : user.nameAr;
    }
    else
      return '';
  }

  subs = new Subscription();

  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }

  constructor(public rightDrawer: RightDrawer,
    private fb: FormBuilder,
    private roleService: RolesService,
    private systemPagesService: SystemPagesService,
    private companyRoleActionsService: CompanyRoleActionsService,
    private companyUsersService: CompanyUsersService,
    private companyUserRolesService: CompanyUserRolesService,
    private snackBar: MatSnackBar,
    private loadingService: LoadingService,
    public currentUserService: CurrentUserService,
    private translateService: TranslateService,
    private languageService: LanguageService) {
    this.roleId = this.rightDrawer.parentData;
    this.currentLang = this.languageService.lang;
  }

  ngOnInit(): void {
    this.initRoleForm();

    //if edit role
    if (this.roleId) this.patchRoleForm();
  }

  initRoleForm() {
    this.companyRoleForm = this.fb.group({
      companyRoleId: new FormControl(null),
      companyId: new FormControl(this.currentUserService.currentUser.companyId),
      roleEnglishName: new FormControl(null, [Validators.required]),
      roleArabicName: new FormControl(null, [Validators.required]),
    });
  }

  patchRoleForm() {
    this.subs.add(
      this.roleService.getRoleById(this.roleId!).subscribe(({ ok, result, message }) => {
        if (result) {
          this.companyRoleForm.patchValue({
            companyRoleId: result.companyRoleId,
            companyId: result.companyId,
            roleEnglishName: result.roleEnglishName,
            roleArabicName: result.roleArabicName
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

  onSaveRole(stepper: MatStepper) {
    this.loading = true;
    this.loadingService.itIsLoading = true;

    // add role
    if (!this.roleId) {
      this.subs.add(
        this.roleService.postRole(this.companyRoleForm.value).subscribe(({ ok, result, message }) => {
          if (result) {
            this.roleId = result.companyRoleId;
            message = this.currentLang == 'en' ? 'Saved successfully !' : 'تم الحفظ بنجاح !';
            this.snackBar.open(message, 'OK', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-success']
            })
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
          stepper.next();
          this.populatePagesActions();
        })
      );
    }
    // edit role
    else {
      this.subs.add(
        this.roleService.putRole(this.companyRoleForm.value).subscribe(({ ok, result, message }) => {
          if (result) {
            message = this.currentLang == 'en' ? 'Saved successfully !' : 'تم الحفظ بنجاح !';
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
          this.loading = false;
          stepper.next();
          this.populatePagesActions();
        })
      );
    }
  }

  populatePagesActions() {
    this.subs.add(
      this.systemPagesService.getPagesWithActionsByRole(this.roleId!).subscribe(({ ok, result, message }) => {
        if (result) {
          this.allPagesActions = result;
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

  checkSelectedActions() {
    let isAnySelectedAction: boolean = this.allPagesActions.some(page => page.systemPageActions.some(action => action.isSelected));
    if (isAnySelectedAction)
      return true;
    else
      return false;
  }

  onSaveActions(stepper: MatStepper) {
    this.loading = true;
    this.loadingService.itIsLoading = true;

    var model: UpdatePagesActionsInRoles = {
      pageActionsInRoles: []
    };
    this.allPagesActions.forEach(page => {
      page.systemPageActions.forEach(action => {
        if (action.isSelected) {
          let roleAction: CompanyRoleAction = {
            companyRoleId: this.roleId!,
            systemPageActionId: action.systemPageActionId
          }
          model.pageActionsInRoles.push(roleAction);
        }
      })
    })

    this.subs.add(
      this.companyRoleActionsService.updatePagesActionsInRoles(model).subscribe(({ ok, result, message }) => {
        if (result) {
          message = this.currentLang == 'en' ? 'Saved successfully !' : 'تم الحفظ بنجاح !';
          this.snackBar.open(message, 'OK', {
            duration: 3000,
            horizontalPosition: 'start',
            panelClass: ['bg-success']
          })
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
        stepper.next();
        this.populateUsers();
      })
    );
  }

  populateUsers() {
    this.usersFilter.companyId = this.currentUserService.currentUser.companyId;
    this.usersFilter.companyRoleId = this.roleId;
    this.subs.add(
      this.companyUsersService.searchCompanyUsers(this.usersFilter).subscribe(({ ok, result, message }) => {
        if (ok) {
          this.allCompanyUsers = result.result;
          this.companyUsers = this.allCompanyUsers.filter(u => u.isSelected);
          this.filteredOptions = this.usersControl.valueChanges
            .pipe(
              startWith(''),
              map(value => typeof value === 'string' ? value : value.name),
              map(name => name ? this._filter(name) : this.allCompanyUsers?.slice())
            );
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

  onSearchUsers(searchText: string) {
    if (searchText.length > 0)
      this.companyUsers = this.allCompanyUsers.filter(user =>
        user.nameAr?.includes(searchText) ||
        user.nameEn?.includes(searchText) ||
        user.email?.includes(searchText) ||
        user.phone?.includes(searchText)
      );
    else this.companyUsers = this.allCompanyUsers;
  }

  onSelectedUser(user: CompanyUser) {
    if (!this.companyUsers.some(u => u == user)) {
      user.isSelected = true;
      if (user.currentRole?.companyRoleId && user.currentRole.companyRoleId != this.roleId) {
        Swal.fire({
          title: this.translateService.instant('managementRoles.form.confirm'),
          text: this.translateService.instant('managementRoles.form.confirmText'),
          icon: 'warning',
          showCancelButton: true,
          backdrop: false,
          confirmButtonColor: '#ffc529',
          confirmButtonText: this.translateService.instant('managementRoles.form.continue'),
          cancelButtonText: this.translateService.instant('managementRoles.form.cancel'),
        }).then((result: any) => {
          if (result.value) {
            user.currentRole = this.companyRoleForm.value;
            this.companyUsers?.length ? this.companyUsers.push(user) : this.companyUsers = [user];
          }
          else
            user.isSelected = false;
        });
      } else {
        user.currentRole = this.companyRoleForm.value;
        this.companyUsers?.length ? this.companyUsers.push(user) : this.companyUsers = [user];
      }
    }
  }

  checkSelectedUsers() {
    let isAnySelectedUser: boolean = this.companyUsers.some(user => user.isSelected);
    if (isAnySelectedUser)
      return true;
    else
      return false;
  }

  onDeleteUser(user: CompanyUser) {
    user.isSelected = false;
    //delete this.companyUsers[this.companyUsers.findIndex(u => u == user)];
    this.companyUsers = this.companyUsers.filter(u => u != user);
  }

  onSaveUsers() {
    this.loading = true;
    this.loadingService.itIsLoading = true;

    var model: UpdateUserRoles = {
      usersRoles: []
    };
    this.companyUsers.forEach(user => {
      if (user.isSelected) {
        let userRole: CompanyUserRole = {
          companyUserId: user.companyUserId,
          companyRoleId: this.roleId!
        }
        model.usersRoles.push(userRole);
      }
    })

    this.subs.add(
      this.companyUserRolesService.updateUsersRoles(model).subscribe(({ ok, result, message }) => {
        if (result) {
          message = this.currentLang == 'en' ? 'Saved successfully !' : 'تم الحفظ بنجاح !';
          this.snackBar.open(message, 'OK', {
            duration: 3000,
            horizontalPosition: 'start',
            panelClass: ['bg-success']
          })
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
        this.rightDrawer.close();
      })
    );
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}
