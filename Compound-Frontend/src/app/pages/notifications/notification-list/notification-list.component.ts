import { Notification, NotificationsService } from './../../../services/api/admin/notifications.service';
import { AfterViewInit, ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ICompound } from 'src/app/services/api/admin/compounds.service';
import { NotificationFilter } from 'src/app/services/api/admin/notifications.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { CompoundsStoreService } from 'src/app/services/compounds-store.serives';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { NotificationFormComponent } from '../notification-form/notification-form.component';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';
import { CurrentUserService } from './../../../services/current-user.serives';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';


@Component({
  selector: 'app-notification-list',
  templateUrl: './notification-list.component.html',
  styleUrls: ['./notification-list.component.scss']
})
export class NotificationListComponent implements OnInit, AfterViewInit, OnDestroy {
  displayedColumns = [
    'position',
    'title',
    'message',
    'toUnitsCount',
    'isOwnerOnly',
    'actions',
  ];
  selectedCompound: ICompound | undefined

  notificationFilter = new NotificationFilter({
    pageNumber: 1,
    pageSize: 5,
    sortBy: 'publishDate',
    isSortAscending: false
  });
  totalItems!: number;
  dataSource!: MatTableDataSource<Notification>;
  @ViewChild(MatSort) sort!: MatSort;
  searchText?: string;

  subs = new Subscription();

  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }

  constructor(private activatedRoute: ActivatedRoute,
    private compoundsStoreService: CompoundsStoreService,
    private notificationService: NotificationsService,
    public currentUserService: CurrentUserService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    private cdr: ChangeDetectorRef,
    public rightDrawer: RightDrawer,
    private loadingService: LoadingService,
    public languageService: LanguageService) { }

  ngOnInit() {
    this.activatedRoute.params.subscribe(({ compoundId }) => {
      this.selectedCompound = this.compoundsStoreService.
        compounds?.find(i => i.compoundId == compoundId);
      this.notificationFilter.compoundId = compoundId;
      this.populateNotification();
    })
  }

  ngAfterViewInit() {
    this.cdr.detectChanges();
    this.subs.add(
      this.sort.sortChange.subscribe(
        (result: any) => {
          if (result.active == "title") {
            this.notificationFilter.sortBy = this.languageService.lang == 'en' ? 'englishTitle' : 'arabicTitle'
          }
          else if (result.active == "message") {
            this.notificationFilter.sortBy = this.languageService.lang == 'en' ? 'englishMessage' : 'arabicMessage'
          }
          else {
            this.notificationFilter.sortBy = result.active;
          }
          this.notificationFilter.isSortAscending = result.direction == 'asc';
          this.populateNotification();
        }
      )
    );
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  populateNotification() {
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.notificationService.getFilteredNotifications(this.notificationFilter).subscribe(({ ok, result, message }) => {
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
    this.notificationFilter.searchText = this.searchText?.trim();
    this.notificationFilter.pageNumber = 1;
    this.totalItems = 0;
    this.populateNotification();
  }

  onOpenForm(compoundNotificationId: string | null = null) {
    this.subs.add(this.rightDrawer.open(NotificationFormComponent, compoundNotificationId).subscribe((arg: any) => {
        this.ngOnInit();
    }));
  }

  openDialog(event: any, notification: Notification) {
    let targetAttr = event.target.getBoundingClientRect();

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    if (this.languageService.lang == 'en') {
      dialogConfig.data = {
        massage: "Are you sure to delete this notification?"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width - 80 + "px"
      };
    } else {
      dialogConfig.data = {
        massage: "هل أنت متأكد من حذف هذا الإشعار"
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
          this.notificationService.deleteNotification(notification.compoundNotificationId).subscribe(({ ok, result, message }) => {
            if (ok) {
              this.dataSource.data.splice(this.dataSource.data.indexOf(notification), 1);
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
    this.notificationFilter.pageNumber = page + 1;
    this.populateNotification();
  }
}
