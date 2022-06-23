import { OwnerFormComponent } from './../owner-form/owner-form.component';
import { AfterViewInit, ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { CompoundOwnersService, Owner, OwnersFilter } from 'src/app/services/api/admin/compound-owners.service';
import { ICompound } from 'src/app/services/api/admin/compounds.service';
import { CompoundsStoreService } from 'src/app/services/compounds-store.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';
import { CurrentUserService } from './../../../services/current-user.serives';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';

@Component({
  selector: 'app-owner-list',
  templateUrl: './owner-list.component.html',
  styleUrls: ['./owner-list.component.scss']
})
export class OwnerListComponent implements OnInit, AfterViewInit, OnDestroy {
  displayedColumns = [
    'position',
    'name',
    'phone',
    'creationDate',
    'units',
    'status',
    'actions',
  ];
  selectedCompound: ICompound | undefined

  ownersFilter = new OwnersFilter({
    pageNumber: 1,
    pageSize: 5,
    sortBy: 'creationDate',
    isSortAscending: false
  });
  totalItems!: number;
  dataSource!: MatTableDataSource<Owner>;
  @ViewChild(MatSort) sort!: MatSort;
  searchText?: string;

  subs = new Subscription();
  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }

  constructor(private activatedRoute: ActivatedRoute,
    private compoundsStoreService: CompoundsStoreService,
    private ownersService: CompoundOwnersService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    public currentUserService: CurrentUserService,
    private cdr: ChangeDetectorRef,
    public rightDrawer: RightDrawer,
    private loadingService: LoadingService,
    public languageService: LanguageService) { }

  ngOnInit() {
    this.activatedRoute.params.subscribe(({ compoundId }) => {
      this.selectedCompound = this.compoundsStoreService.
        compounds?.find(i => i.compoundId == compoundId);
      this.ownersFilter.compoundId = compoundId;
      this.populateOwners();
    })
  }

  ngAfterViewInit() {
    this.cdr.detectChanges();
    this.subs.add(
      this.sort.sortChange.subscribe(
        (result: any) => {
          this.ownersFilter.sortBy = result.active;
          this.ownersFilter.isSortAscending = result.direction == 'asc';
          this.populateOwners();
        }
      )
    );
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  populateOwners() {
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.ownersService.getFilteredOwners(this.ownersFilter).subscribe(({ ok, result, message }) => {
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

  onSearch() {
    this.ownersFilter.searchText = this.searchText?.trim();
    this.ownersFilter.pageNumber = 1;
    this.totalItems = 0;
    this.populateOwners();
  }

  onOpenForm(ownerId: string) {
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.ownersService.getCompoundOwnerById(ownerId).subscribe(({ ok, result, message }) => {
        if (ok) {
          this.rightDrawer.open(OwnerFormComponent, result).subscribe((arg: any) => {
            this.ngOnInit();
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

  onChangeStatus(owner: Owner) {
    this.loadingService.itIsLoading = true;
    owner.isActive = !owner.isActive;
    this.subs.add(
      this.ownersService.putCompoundOwner(owner).subscribe(({ ok, result, message }) => {
        if (ok) {
          if (owner.isActive) {
            message = this.languageService.lang == 'en' ? 'Activated Successfully !' : 'تم التفعيل بنجاح !';
          } else {
            message = this.languageService.lang == 'en' ? 'Deactivated successfully !' : 'تم التعطيل بنجاح !';
          }
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

  openDialog(event: any, owner: Owner) {
    let targetAttr = event.target.getBoundingClientRect();

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    if (this.languageService.lang == 'en') {
      dialogConfig.data = {
        massage: "Are you sure to delete this owner?"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width - 80 + "px"
      };
    } else {
      dialogConfig.data = {
        massage: "هل أنت متأكد من حذف هذا المالك"
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
          this.ownersService.DeleteCompoundOwner(owner.compoundOwnerId).subscribe(({ ok, result, message }) => {
            if (ok) {
              this.dataSource.data.splice(this.dataSource.data.indexOf(owner), 1);
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
    this.ownersFilter.pageNumber = page + 1;
    this.populateOwners();
  }
}
