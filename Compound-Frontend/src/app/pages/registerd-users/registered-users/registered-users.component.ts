import { Subscription } from 'rxjs';
import { SelectionModel } from '@angular/cdk/collections';
import { HttpParams } from '@angular/common/http';
import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { CompoundsService } from 'src/app/services/api/admin/compounds.service';
import { OwnerRegistrationsService } from 'src/app/services/api/admin/owner-registrations.service';
import { CompoundsStoreService } from 'src/app/services/compounds-store.serives';
import { CurrentUserService } from 'src/app/services/current-user.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';
import { FormOwnerRegistrationComponent } from '../form-owner-registration/form-owner-registration.component';
import { PagedInput } from 'src/app/services/webApi.service';
import { ActivatedRoute } from '@angular/router';

import { SystemPageActions } from 'src/app/services/system-page-actions.enum';




@Component({
  selector: 'app-registered-users',
  templateUrl: './registered-users.component.html',
  styleUrls: ['./registered-users.component.scss']
})
export class RegisteredUsersComponent implements OnInit, OnDestroy {
  displayedColumns: string[] = ['position', 'image', 'phone', 'email', 'userConfirmed', 'action'];
  dataSource = new MatTableDataSource<any>();
  selection = new SelectionModel<any>(true, []);
  loading: boolean = false;
  actionLoading: boolean = false;
  totalItems!: number;
  listPaging = new PagedInput({
    pageNumber: 1,
    pageSize: 5
  });
  statusParam?:any;

  subs = new Subscription();

  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }


  constructor(
    public rightDrawer: RightDrawer,
    private compoundsService: CompoundsService,
    private compoundsStoreService: CompoundsStoreService,
    private snackBar: MatSnackBar,
    public languageService: LanguageService,
    public dialog: MatDialog,
    public currentUserService: CurrentUserService,
    private ownerRegistrationsService: OwnerRegistrationsService,
    public loadingService: LoadingService,
    private activatedRoute: ActivatedRoute,
   
  ) { }

  ngOnInit() {
    this.populateOwnerRegistrations();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  populateOwnerRegistrations(name?: string, phone?: string) {
    this.loadingService.itIsLoading = true;
    this.loading = true;
    let compoundsIds = this.compoundsStoreService.compounds.map(compound => { return compound.compoundId })
    let filterParams = new HttpParams().set('userType', '1')
      .set('compounds', compoundsIds.toString())
      .set('pageNumber', this.listPaging.pageNumber.toString())
      .set('pageSize', this.listPaging.pageSize.toString());
    if (name) filterParams = filterParams.set('name', name);
    if (phone) filterParams = filterParams.set('phone', phone);
    this.statusParam = this.activatedRoute.snapshot.queryParamMap.get('userConfirmed');
    if (this.statusParam != null) filterParams = filterParams.set('userConfirmed', this.statusParam);
    this.subs.add(
      this.ownerRegistrationsService.getFilteredOwnerRegistrations(filterParams
      ).subscribe(({ ok, result }) => {
        if (ok) {
          this.dataSource = new MatTableDataSource(result.result);
          this.loadingService.itIsLoading = false;
          this.loading = false;
          this.totalItems = result.totalCount;
        }
      }));
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    if (filterValue) {
      this.listPaging.pageNumber = 1;
      this.totalItems = 0;
      this.dataSource.filter = filterValue.trim().toLowerCase();
      // if filter text is for name string
      if (isNaN(Number(filterValue))) {
        this.populateOwnerRegistrations(filterValue.trim().toLowerCase());
      }
      // if filter text is for mobile number
      else {
        this.populateOwnerRegistrations(undefined, filterValue.trim());
      }
    } else {
      this.populateOwnerRegistrations();
    }
  }

  openDialog(event: any) {
    let targetAttr = event.target.getBoundingClientRect();

    //obj.action=action;
    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    dialogConfig.data = {
      // obj: obj,
      name: "User Account"
    };
    dialogConfig.position = {
      top: targetAttr.y + targetAttr.height - 130 + "px",
      left: targetAttr.x - targetAttr.width - 80 + "px"
    };
    dialogConfig.backdropClass = 'dialog-bg-trans';
    const dialogRef = this.dialog.open(DialogLayoutComponent, dialogConfig
    );
    dialogRef.afterClosed().subscribe(result => {
      this.compoundsService.DeleteCompounds(result.data.obj.compoundId).subscribe(data => {
        data = result.data.obj.compoundId;
      })
    });
  }
  confirmUser(ownerRegistration: any) {
    this.loadingService.itIsLoading = true;
    this.ownerRegistrationsService.getOwnerRegistrations(
      new HttpParams()
        .set('phone', ownerRegistration.phone)
        .set('companyId', this.currentUserService.currentUser.companyId)
    ).subscribe(({ ok, result, message }) => {
      if (ok) {
        this.loadingService.itIsLoading = false;
        this.rightDrawer.open(FormOwnerRegistrationComponent, result).subscribe((arg: any) => {
          if (arg != null) {
            this.ngOnInit();
          }
        });
      } else {
        this.snackBar.open(message, 'ERROR', {
          duration: 3000,
          horizontalPosition: 'start',
          panelClass: ['bg-danger']
        })
      }
    });
  }
  cancelUser() {

  }

  onPageChange(page: number) {
    this.listPaging.pageNumber = page + 1;
    this.populateOwnerRegistrations();
  }
}






