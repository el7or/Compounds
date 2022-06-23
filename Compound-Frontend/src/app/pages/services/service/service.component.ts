import { LanguageService } from 'src/app/services/language.serives';
import { ServiceRequest, ServiceRequestsFilter, CompoundServicesService, ServiceStatus } from './../../../services/api/admin/compound-services.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { Subscription } from 'rxjs';
import { ICompound } from 'src/app/services/api/admin/compounds.service';
import { ActivatedRoute } from '@angular/router';
import { CompoundsStoreService } from 'src/app/services/compounds-store.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';
import { CurrentUserService } from 'src/app/services/current-user.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { ServiceRequestComponent } from '../service-request/service-request.component';

@Component({
  selector: 'app-service',
  templateUrl: './service.component.html',
  styleUrls: ['./service.component.scss']
})
export class ServiceComponent implements OnInit {
  subs = new Subscription();
  selectedCompound: ICompound | undefined
  displayedColumns = [
    'requestNumber',
    'requestedBy',
    'serviceType',
    'serviceSubType',
    'status',
    'postDate',
    //'postTime',
    'from',
    'to',
    'rate',
    'actions'
  ];
  requestsFilter = new ServiceRequestsFilter({
    pageNumber: 1,
    pageSize: 5,
    sortBy: 'postDate',
    isSortAscending: false
  });
  totalItems!: number;
  dataSource!: MatTableDataSource<ServiceRequest>;
  @ViewChild(MatSort) sort!: MatSort;
  searchText?: string;
  statusParam!: any;

  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }

  constructor(public languageService: LanguageService,
    private activatedRoute: ActivatedRoute,
    private compoundsStoreService: CompoundsStoreService,
    private compoundServicesService: CompoundServicesService,

    private loadingService: LoadingService,
    private snackBar: MatSnackBar,
    public currentUserService: CurrentUserService,
    private cdr: ChangeDetectorRef,
    public rightDrawer: RightDrawer,
    private dialog: MatDialog) {
    this.statusParam = this.activatedRoute.snapshot.queryParamMap.get('status');
    if (this.statusParam)
      this.requestsFilter.status = this.statusParam;
  }

  ngOnInit() {
    this.activatedRoute.params.subscribe(({ compoundId }) => {
      this.selectedCompound = this.compoundsStoreService.
        compounds?.find(i => i.compoundId == compoundId);
      this.requestsFilter.compoundId = compoundId;
      this.requestsFilter.serviceTypeIds = this.currentUserService.currentUser?.accessDetails?.compounds?.find(c => c.compoundId == compoundId)?.serviceTypesIds;
      this.populateServices();
    });
  }

  ngAfterViewInit() {
    this.cdr.detectChanges();
    this.subs.add(
      this.sort.sortChange.subscribe(
        (result: any) => {
          if (result.active == "serviceType") {
            this.requestsFilter.sortBy = this.languageService.lang == 'en' ? 'serviceTypeEnglish' : 'serviceTypeArabic'
          }
          else {
            this.requestsFilter.sortBy = result.active;
          }
          this.requestsFilter.isSortAscending = result.direction == 'asc';
          this.populateServices();
        }
      )
    );
  }

  populateServices() {
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.compoundServicesService.getFilteredRequests(this.requestsFilter).subscribe(({ ok, result, message }) => {
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
    this.requestsFilter.searchText = this.searchText?.trim();
    this.requestsFilter.pageNumber = 1;
    this.totalItems = 0;
    this.populateServices();
  }

  openCancelDialog(event: any, request: ServiceRequest) {
    let targetAttr = event.target.getBoundingClientRect();

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    if (this.languageService.lang == 'en') {
      dialogConfig.data = {
        massage: "Are you sure to cancel this request?"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width - 80 + "px"
      };
    } else {
      dialogConfig.data = {
        massage: "هل أنت متأكد من إلغاء هذا الطلب"
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
          this.compoundServicesService.updateRequestStatus(request.serviceRequestId, { status: 2 })
            .subscribe(({ ok, result, message }) => {
              if (ok) {
                message = this.languageService.lang == 'en' ? 'Cancelled Successfully !' : 'تم الإلغاء بنجاح !';
                this.snackBar.open(message, 'OK', {
                  duration: 3000,
                  horizontalPosition: 'start',
                  panelClass: ['bg-success']
                });
                this.populateServices();
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

  onOpenForm(serviceRequestId: string | null = null) {
    this.subs.add(this.rightDrawer.open(ServiceRequestComponent, serviceRequestId).subscribe((arg: any) => {
      this.ngOnInit();
    }));
  }

  openDoneDialog(event: any, request: ServiceRequest) {
    let targetAttr = event.target.getBoundingClientRect();

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    if (this.languageService.lang == 'en') {
      dialogConfig.data = {
        massage: "Are you sure to mark this request as done?"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width - 80 + "px"
      };
    } else {
      dialogConfig.data = {
        massage: "هل أنت متأكد من تحديد هذا الطلب بأنه تم إنجازه ؟"
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
          this.compoundServicesService.updateRequestStatus(request.serviceRequestId, { status: 1 })
            .subscribe(({ ok, result, message }) => {
              if (ok) {
                message = this.languageService.lang == 'en' ? 'Done Successfully !' : 'تم تعديل الطلب بنجاح !';
                this.snackBar.open(message, 'OK', {
                  duration: 3000,
                  horizontalPosition: 'start',
                  panelClass: ['bg-success']
                });
                this.populateServices();
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

  onChangeStatus(news: any) {

  }

  onPageChange(page: number) {
    this.requestsFilter.pageNumber = page + 1;
    this.populateServices();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

}
