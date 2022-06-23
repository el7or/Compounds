import { RoleFormComponent } from './../role-form/role-form.component';
import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { Subscription } from 'rxjs';
import { Role, RolesFilter, RolesService } from 'src/app/services/api/admin/roles.service';
import { CurrentUserService } from 'src/app/services/current-user.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';


@Component({
  selector: 'app-role-list',
  templateUrl: './role-list.component.html',
  styleUrls: ['./role-list.component.scss']
})
export class RoleListComponent implements OnInit, OnDestroy {
  displayedColumns = [
    'position',
    'roleArabicName',
    'roleEnglishName',
    'pagesCount',
    'usersCount',
    'actions'
  ];

  roleFilter = new RolesFilter({
    pageNumber: 1,
    pageSize: 5,
    sortBy: 'publishDate',
    isSortAscending: false
  });
  totalItems!: number;
  dataSource!: MatTableDataSource<Role>;
  //searchText?: string;

  subs = new Subscription();

  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }

  constructor(
    private roleService: RolesService,
    private dialog: MatDialog,
    public currentUserService: CurrentUserService,
    public rightDrawer: RightDrawer,
    private snackBar: MatSnackBar,
    private loadingService: LoadingService,
    public languageService: LanguageService) { }

  ngOnInit() {
    this.roleFilter.companyId = this.currentUserService.currentUser.companyId;
    this.populateRoles();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  populateRoles() {
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.roleService.getRolesByCompanyId(this.roleFilter).subscribe(({ ok, result, message }) => {
        if (result) {
          this.dataSource = new MatTableDataSource(result);
          this.totalItems = result.totalCount;
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

  // onSearch() {
  //   this.roleFilter.searchText = this.searchText?.trim();
  //   this.roleFilter.pageNumber = 1;
  //   this.totalItems = 0;
  //   this.populateRoles();
  // }

  onOpenForm(roleId: string | null = null) {
    this.subs.add(this.rightDrawer.open(RoleFormComponent, roleId).subscribe((arg: any) => {
      this.ngOnInit();
    }));
  }

  openDialog(event: any, role: Role) {
    let targetAttr = event.target.getBoundingClientRect();

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    if (this.languageService.lang == 'en') {
      dialogConfig.data = {
        massage: "Are you sure to delete this role?"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width - 80 + "px"
      };
    } else {
      dialogConfig.data = {
        massage: "هل أنت متأكد من حذف هذا الدور؟"
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
          this.roleService.deleteRole(role.companyRoleId).subscribe(({ ok, result, message }) => {
            if (ok) {
              this.dataSource.data.splice(this.dataSource.data.indexOf(role), 1);
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

  onPageChange(page: number) {
    this.loadingService.itIsLoading = true;
    this.roleFilter.pageNumber = page + 1;
    this.populateRoles();
  }
}
