import { PendingListItem, PendingType } from './../../../services/api/admin/compounds.service';
import { Subscription } from 'rxjs';
import { CurrentUserService } from './../../../services/current-user.serives';
import { Component, OnInit, OnDestroy, Input, ElementRef, ViewChild, AfterViewChecked } from '@angular/core';
import { ICompound, CompoundsService, AllPendingsFilter } from 'src/app/services/api/admin/compounds.service';
import { CompoundsStoreService } from 'src/app/services/compounds-store.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { NotificationDrawerService } from 'src/app/services/notification-drawer.service';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';
import { LoadingService } from 'src/app/services/loading.serives';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { ServiceRequestComponent } from 'src/app/pages/services/service-request/service-request.component';
import { IssueRequestComponent } from 'src/app/pages/issues/issue-request/issue-request.component';
import { VisitsService } from 'src/app/services/api/admin/visits.service';
import { VisitViewComponent } from 'src/app/pages/visits/visit-view/visit-view.component';
import { OwnerRegistrationsService } from 'src/app/services/api/admin/owner-registrations.service';
import { HttpParams } from '@angular/common/http';
import { FormOwnerRegistrationComponent } from 'src/app/pages/registerd-users/form-owner-registration/form-owner-registration.component';
import { SignalRService } from 'src/app/services/signal-r.service';

@Component({
  selector: 'app-notification-drawer',
  templateUrl: './notification-drawer.component.html',
  styleUrls: ['./notification-drawer.component.scss']
})
export class NotificationDrawerComponent implements OnInit, AfterViewChecked, OnDestroy {
  compounds: ICompound[] | undefined;
  selectedCompound: ICompound | null = null;
  allPendingsFilter = new AllPendingsFilter({
    pageNumber: 1,
    pageSize: 10
  });
  pendingList!: PendingListItem[];
  loading: boolean = false;
  @ViewChild('loadingMoreElRef') loadingMoreElRef!: ElementRef;
  loadingMoreObserver!: IntersectionObserver;

  subs = new Subscription();

  public get PendingType(): typeof PendingType {
    return PendingType;
  }

  constructor(
    public notificationDrawerService: NotificationDrawerService,
    private compoundsStoreService: CompoundsStoreService,
    private compoundsService: CompoundsService,
    private currentUserService: CurrentUserService,
    private loadingService: LoadingService,
    private snackBar: MatSnackBar,
    private rightDrawer: RightDrawer,
    private visitsService: VisitsService,
    private ownerRegistrationsService: OwnerRegistrationsService,
    public languageService: LanguageService,
    private signalRService: SignalRService
  ) {
    this.signalRService.updatePendingListCount.subscribe((data: boolean) => {
      if (data)
        this.setPendingsListCount();
    });
  }

  ngOnInit(): void {
    this.compounds = this.compoundsStoreService.compounds;
    this.allPendingsFilter.compoundsIds = this.compounds.map(c => c.compoundId);
    this.allPendingsFilter.isShowUsers = this.currentUserService.checkRolePageAction(SystemPageActions.RegisteredUsersConfirmed);
    this.allPendingsFilter.isShowVisits = this.currentUserService.checkRolePageAction(SystemPageActions.VisitConfirm);
    this.allPendingsFilter.isShowServices = this.currentUserService.checkRolePageAction(SystemPageActions.ServicesMarkasdone);
    this.allPendingsFilter.isShowIssues = this.currentUserService.checkRolePageAction(SystemPageActions.IssuesMarkasdone);

    this.populatePendingsList();
    this.scrollLoadingMoreInit();
  }

  ngAfterViewChecked() {
    this.loadingMoreObserver.observe(this.loadingMoreElRef.nativeElement);
  }

  populatePendingsList() {
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.compoundsService.getAllPendings(this.allPendingsFilter)
        .subscribe(({ ok, result, message }) => {
          if (ok) {
            this.notificationDrawerService.badge = result.totalCount;
            this.allPendingsFilter.pageNumber == 1 ? this.pendingList = result.result : this.pendingList.push(...result.result);
            this.pendingList = this.pendingList.map((item: PendingListItem) => {
              switch (item.pendingType) {
                case PendingType.Service:
                  item.icon = 'miscellaneous_services';
                  break;
                case PendingType.Issue:
                  item.icon = 'report_problem';
                  break;
                case PendingType.Visit:
                  item.icon = 'meeting_room';
                  break;
                case PendingType.User:
                  item.icon = 'how_to_reg';
                  break;
              }
              return item;
            });
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

  setPendingsListCount() {
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.compoundsService.getAllPendingsCount(this.allPendingsFilter)
        .subscribe(({ ok, result, message }) => {
          if (ok) {
            this.notificationDrawerService.badge = result;
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

  selectCompound(compound: ICompound | null) {
    this.selectedCompound = compound;
    if (compound == null) {
      this.allPendingsFilter.compoundsIds = this.compounds!.map(c => c.compoundId);
    }
    else {
      this.allPendingsFilter.compoundsIds = [compound?.compoundId];
    }
    this.allPendingsFilter.pageNumber = 1;
    this.allPendingsFilter.pageSize = 10;
    this.populatePendingsList();
  }

  onOpenDetails(item: PendingListItem) {
    switch (item.pendingType) {
      case PendingType.Service:
        this.subs.add(this.rightDrawer.open(ServiceRequestComponent, item.id).subscribe((arg: any) => {
          this.ngOnInit();
        }));
        break;
      case PendingType.Issue:
        this.subs.add(this.rightDrawer.open(IssueRequestComponent, item.id).subscribe((arg: any) => {
          this.ngOnInit();
        }));
        break;
      case PendingType.Visit:
        this.loadingService.itIsLoading = true;
        this.subs.add(
          this.visitsService.getVisit(item.id).subscribe(({ ok, result, message }) => {
            if (ok) {
              this.rightDrawer.open(VisitViewComponent, result).subscribe((arg: any) => {
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
            this.loadingService.itIsLoading = false;
          })
        );
        break;
      case PendingType.User:
        this.loadingService.itIsLoading = true;
        this.ownerRegistrationsService.getOwnerRegistrations(
          new HttpParams()
            .set('phone', item.ownerRegistrationPhone!)
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
        break;
    }
  }

  closeDrawer() {
    this.selectCompound(null);
    this.notificationDrawerService.close();
  }

  scrollLoadingMoreInit() {
    this.loadingMoreObserver = new IntersectionObserver((entries) => {
      if (entries[0].isIntersecting === true && this.pendingList?.length) {
        this.onLoadMore();
      }
    }, { threshold: [0] });
  }

  onLoadMore() {
    this.loadingService.itIsLoading = true;
    this.loading = true;
    this.allPendingsFilter.pageNumber = this.allPendingsFilter.pageNumber + 1;
    this.populatePendingsList();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}
