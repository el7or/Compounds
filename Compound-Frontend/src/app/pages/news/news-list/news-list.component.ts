import { CurrentUserService } from './../../../services/current-user.serives';
import { NewsFormComponent } from './../news-form/news-form.component';
import { Subscription } from 'rxjs';
import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';

import { LanguageService } from 'src/app/services/language.serives';
import { News, NewsFilter, NewsService } from './../../../services/api/admin/news.service';
import { ICompound } from 'src/app/services/api/admin/compounds.service';
import { CompoundsStoreService } from 'src/app/services/compounds-store.serives';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';
import { LoadingService } from 'src/app/services/loading.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';

@Component({
  selector: 'app-news-list',
  templateUrl: './news-list.component.html',
  styleUrls: ['./news-list.component.scss']
})
export class NewsListComponent implements OnInit, AfterViewInit, OnDestroy {
  displayedColumns = [
    'position',
    'title',
    'summary',
    'publishDate',
    'foregroundTillDate',
    'status',
    'actions',
  ];
  selectedCompound: ICompound | undefined

  newsFilter = new NewsFilter({
    pageNumber: 1,
    pageSize: 5,
    sortBy: 'publishDate',
    isSortAscending: false
  });
  totalItems!: number;
  dataSource!: MatTableDataSource<News>;
  @ViewChild(MatSort) sort!: MatSort;
  searchText?: string;

  subs = new Subscription();

  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }

  constructor(private activatedRoute: ActivatedRoute,
    private compoundsStoreService: CompoundsStoreService,
    public currentUserService: CurrentUserService,
    private newsService: NewsService,
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
      this.newsFilter.compoundId = compoundId;
      this.populateNews();
    })
  }

  ngAfterViewInit() {
    this.cdr.detectChanges();
    this.subs.add(
      this.sort.sortChange.subscribe(
        (result: any) => {
          if (result.active == "title") {
            this.newsFilter.sortBy = this.languageService.lang == 'en' ? 'englishTitle' : 'arabicTitle'
          }
          else if (result.active == "summary") {
            this.newsFilter.sortBy = this.languageService.lang == 'en' ? 'englishSummary' : 'arabicSummary'
          }
          else {
            this.newsFilter.sortBy = result.active;
          }
          this.newsFilter.isSortAscending = result.direction == 'asc';
          this.populateNews();
        }
      )
    );
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  populateNews() {
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.newsService.getFilteredNews(this.newsFilter).subscribe(({ ok, result, message }) => {
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
    this.newsFilter.searchText = this.searchText?.trim();
    this.newsFilter.pageNumber = 1;
    this.totalItems = 0;
    this.populateNews();
  }

  onOpenForm(compoundNewsId: string | null = null) {
    this.subs.add(this.rightDrawer.open(NewsFormComponent, compoundNewsId).subscribe((arg: any) => {
        this.ngOnInit();
    }));
  }

  onChangeStatus(news: News) {
    this.loadingService.itIsLoading = true;
    news.isActive = !news.isActive;
    this.subs.add(
      this.newsService.putNews(news).subscribe(({ ok, result, message }) => {
        if (ok) {
          if (news.isActive) {
            message = this.languageService.lang == 'en' ? 'Activated Successfully !' : 'تم تفعيل النشر بنجاح !';
          } else {
            message = this.languageService.lang == 'en' ? 'Deactivated successfully !' : 'تم إلغاء النشر بنجاح !';
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

  openDialog(event: any, news: News) {
    let targetAttr = event.target.getBoundingClientRect();

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    if (this.languageService.lang == 'en') {
      dialogConfig.data = {
        massage: "Are you sure to delete this news?"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width - 80 + "px"
      };
    } else {
      dialogConfig.data = {
        massage: "هل أنت متأكد من حذف هذا الخبر؟"
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
          this.newsService.deleteNews(news.compoundNewsId).subscribe(({ ok, result, message }) => {
            if (ok) {
              this.dataSource.data.splice(this.dataSource.data.indexOf(news), 1);
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
    this.newsFilter.pageNumber = page + 1;
    this.populateNews();
  }
}
