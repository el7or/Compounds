import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { CompanyUser, CompanyUsersService, UsersFilter } from 'src/app/services/api/admin/company-users.service';
import { LanguageService } from 'src/app/services/language.serives';
import { MatTableDataSource } from '@angular/material/table';
import { Subscription } from 'rxjs';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { CompanyUserComponent } from '../company-user/company.user.component';
import { LoadingService } from 'src/app/services/loading.serives';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CurrentUserService } from 'src/app/services/current-user.serives';
import { MatSort } from '@angular/material/sort';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';

@Component({
  selector: 'app-subscription',
  templateUrl: './subscription.component.html',
  styleUrls: ['./subscription.component.scss']
})
export class SubscriptionComponent implements OnInit {
  displayedColumns = [
    'username',
    'name',
    'phone',
    'email',
    'status',
    'actions'
  ];
  searchText?: string;
  totalItems!: number;
  usersFilter = new UsersFilter({
    pageNumber: 1,
    pageSize: 5,
    sortBy: 'name',
    isSortAscending: false
  });
  dataSource!: MatTableDataSource<CompanyUser>;
  subs = new Subscription();
  @ViewChild(MatSort) sort!: MatSort;

  
  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }

  
  constructor(public languageService: LanguageService,
    public rightDrawer: RightDrawer,
    private loadingService: LoadingService,
    private snackBar: MatSnackBar,
    private usersService:CompanyUsersService,
    public currentUserService: CurrentUserService,
    private cdr: ChangeDetectorRef,
    private dialog: MatDialog) { }

  ngOnInit(): void {
    this.populateUses();
  }

  ngAfterViewInit() {
    this.cdr.detectChanges();
    this.subs.add(
      this.sort.sortChange.subscribe(
        (result: any) => {
          if (result.active == "name") {
            this.usersFilter.sortBy = this.languageService.lang == 'en' ? 'nameEn' : 'nameAr'
          }
          else {
            this.usersFilter.sortBy = result.active;
          }
          this.usersFilter.isSortAscending = result.direction == 'asc';
          this.populateUses();
        }
      )
    );
  }

  onSearch() {
    this.usersFilter.searchText = this.searchText?.trim();
    this.usersFilter.pageNumber = 1;
    this.totalItems = 0;
    this.populateUses();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  onPageChange(page: number) {
    this.loadingService.itIsLoading = true;
    this.usersFilter.pageNumber = page + 1;
    this.populateUses();
  }

  onOpenForm(userId: string | null = null) {
    this.subs.add(this.rightDrawer.open(CompanyUserComponent, userId).subscribe((arg: any) => {
      this.ngOnInit();
  }));
  }

  openDialog(event: any, user: any) {
    let targetAttr = event.target.getBoundingClientRect();

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    if (this.languageService.lang == 'en') {
      dialogConfig.data = {
        massage: "Are you sure to delete this user?"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width - 80 + "px"
      };
    } else {
      dialogConfig.data = {
        massage: "هل أنت متأكد من حذف هذا المستخدم؟"
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
          this.usersService.DeleteCompanyUsers(user.companyUserId).subscribe(({ ok, result, message }) => {
            if (ok) {
              this.dataSource.data.splice(this.dataSource.data.indexOf(user), 1);
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

  populateUses() {
    this.loadingService.itIsLoading = true;
    this.usersFilter.companyId=this.currentUserService.currentUser.companyId;
    this.subs.add(
      this.usersService.searchCompanyUsers(this.usersFilter).subscribe(({ ok, result, message }) => {
        if (ok) {
          this.dataSource = new MatTableDataSource(result.result);
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

  changeStatus(event: any, user: any) {
    let targetAttr = event.target.getBoundingClientRect();

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    if (this.languageService.lang == 'en') {
      dialogConfig.data = {
        massage: user.isActive? "Are you sure to deactivate this user?" : "Are you sure to activate this user?"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width - 80 + "px"
      };
    } else {
      dialogConfig.data = {
        massage: user.isActive? "هل تريد تفعيل هذا المستخدم؟":"هل تريد الغاء تفعيل هذا المستخدم؟"
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
        let body={status: !user.isActive};
        this.subs.add(
          this.usersService.changeUserStatus(user.companyUserId,body).subscribe(({ ok, result, message }) => {
            if (ok) {
              message = this.languageService.lang == 'en' ? 'Updated Successfully !' : 'تم التعديل بنجاح !';
              this.snackBar.open(message, 'OK', {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-success']
              });
              this.populateUses();
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
