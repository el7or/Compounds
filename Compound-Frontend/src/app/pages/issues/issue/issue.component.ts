import { RightDrawer } from './../../../services/right-drawer.serives';
import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { IssueRequestsFilter, IssueRequest, CompoundIssuesService } from 'src/app/services/api/admin/compound-issues.service';
import { ICompound } from 'src/app/services/api/admin/compounds.service';
import { CompoundsStoreService } from 'src/app/services/compounds-store.serives';
import { CurrentUserService } from 'src/app/services/current-user.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';
import { IssueRequestComponent } from '../issue-request/issue-request.component';

@Component({
  selector: 'app-issue',
  templateUrl: './issue.component.html',
  styleUrls: ['./issue.component.scss']
})
export class IssueComponent implements OnInit {
  subs = new Subscription();
  selectedCompound: ICompound | undefined
  displayedColumns = [
    'requestNumber',
    'requestedBy',
    'issueType',
    'status',
    'postDate',
    'rate',
    'actions'
  ];
  requestsFilter = new IssueRequestsFilter({
    pageNumber: 1,
    pageSize: 5,
    sortBy: 'postDate',
    isSortAscending: false
  });
  totalItems!: number;
  dataSource!: MatTableDataSource<IssueRequest>;
  @ViewChild(MatSort) sort!: MatSort;
  searchText?: string;
  statusParam!: any;

  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }

  constructor(public languageService: LanguageService,
    private activatedRoute: ActivatedRoute,
    private compoundsStoreService: CompoundsStoreService,
    private compoundIssuesService: CompoundIssuesService,
    public rightDrawer: RightDrawer,
    public currentUserService: CurrentUserService,
    private loadingService: LoadingService,
    private snackBar: MatSnackBar,
    private cdr: ChangeDetectorRef,
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
      this.requestsFilter.issueTypeIds = this.currentUserService.currentUser?.accessDetails?.compounds?.find(c => c.compoundId == compoundId)?.issueTypesIds;
      this.populateIssues();
    });
  }

  ngAfterViewInit() {
    this.cdr.detectChanges();
    this.subs.add(
      this.sort.sortChange.subscribe(
        (result: any) => {
          if (result.active == "issueType") {
            this.requestsFilter.sortBy = this.languageService.lang == 'en' ? 'issueTypeEnglish' : 'issueTypeArabic'
          }
          else {
            this.requestsFilter.sortBy = result.active;
          }
          this.requestsFilter.isSortAscending = result.direction == 'asc';
          this.populateIssues();
        }
      )
    );
  }

  populateIssues() {
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.compoundIssuesService.getFilteredRequests(this.requestsFilter).subscribe(({ ok, result, message }) => {
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

  onOpenForm(issueRequestId: string | null = null) {
    this.subs.add(this.rightDrawer.open(IssueRequestComponent, issueRequestId).subscribe((arg: any) => {
      this.ngOnInit();
    }));
  }

  onSearch() {
    this.requestsFilter.searchText = this.searchText?.trim();
    this.requestsFilter.pageNumber = 1;
    this.totalItems = 0;
    this.populateIssues();
  }

  openCancelDialog(event: any, request: IssueRequest) {
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
          this.compoundIssuesService.updateRequestStatus(request.issueRequestId, { status: 2 })
            .subscribe(({ ok, result, message }) => {
              if (ok) {
                message = this.languageService.lang == 'en' ? 'Cancelled Successfully !' : 'تم الإلغاء بنجاح !';
                this.snackBar.open(message, 'OK', {
                  duration: 3000,
                  horizontalPosition: 'start',
                  panelClass: ['bg-success']
                });
                this.populateIssues();
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

  openDoneDialog(event: any, request: IssueRequest) {
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
          this.compoundIssuesService.updateRequestStatus(request.issueRequestId, { status: 1 })
            .subscribe(({ ok, result, message }) => {
              if (ok) {
                message = this.languageService.lang == 'en' ? 'Done Successfully !' : 'تم تعديل الطلب بنجاح !';
                this.snackBar.open(message, 'OK', {
                  duration: 3000,
                  horizontalPosition: 'start',
                  panelClass: ['bg-success']
                });
                this.populateIssues();
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
    this.populateIssues();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}
