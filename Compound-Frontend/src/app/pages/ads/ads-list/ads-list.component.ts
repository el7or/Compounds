import { Ad, AdsService } from './../../../services/api/admin/ads.service';
import { AfterViewInit, ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ICompound } from 'src/app/services/api/admin/compounds.service';
import { AdsFilter } from 'src/app/services/api/admin/ads.service';
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
import { AdsFormComponent } from '../ads-form/ads-form.component';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';
import { CurrentUserService } from './../../../services/current-user.serives';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';

@Component({
  selector: 'app-ads-list',
  templateUrl: './ads-list.component.html',
  styleUrls: ['./ads-list.component.scss']
})
export class AdsListComponent implements OnInit, AfterViewInit, OnDestroy {
  displayedColumns = [
    'position',
    //'title',
    //'description',
    'startDate',
    'endDate',
    'showsCount',
    'clicksCount',
    'uniqueShowsCount',
    'uniqueClicksCount',
    //'status',
    'actions'
  ];
  selectedCompound: ICompound | undefined

  adFilter = new AdsFilter({
    pageNumber: 1,
    pageSize: 5,
    sortBy: 'publishDate',
    isSortAscending: false
  });
  totalItems!: number;
  dataSource!: MatTableDataSource<Ad>;
  @ViewChild(MatSort) sort!: MatSort;
  searchText?: string;

  subs = new Subscription();

  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }


  constructor(private activatedRoute: ActivatedRoute,
    private compoundsStoreService: CompoundsStoreService,
    private adService: AdsService,
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
      this.adFilter.compoundId = compoundId;
      this.populateAd();
    })
  }

  ngAfterViewInit() {
    this.cdr.detectChanges();
    this.subs.add(
      this.sort.sortChange.subscribe(
        (result: any) => {
          if (result.active == "title") {
            this.adFilter.sortBy = this.languageService.lang == 'en' ? 'englishTitle' : 'arabicTitle'
          }
          else if (result.active == "description") {
            this.adFilter.sortBy = this.languageService.lang == 'en' ? 'englishDescription' : 'arabicDescription'
          }
          else {
            this.adFilter.sortBy = result.active;
          }
          this.adFilter.isSortAscending = result.direction == 'asc';
          this.populateAd();
        }
      )
    );
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  populateAd() {
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.adService.getFilteredAds(this.adFilter).subscribe(({ ok, result, message }) => {
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
    this.adFilter.searchText = this.searchText?.trim();
    this.adFilter.pageNumber = 1;
    this.totalItems = 0;
    this.populateAd();
  }

  onOpenForm(compoundAdId: string | null = null) {
    this.subs.add(this.rightDrawer.open(AdsFormComponent, compoundAdId).subscribe((arg: any) => {
        this.ngOnInit();
    }));
  }

  onChangeStatus(ad: Ad) {
    this.loadingService.itIsLoading = true;
    ad.isActive = !ad.isActive;
    this.subs.add(
      this.adService.putAd(ad).subscribe(({ ok, result, message }) => {
        if (ok) {
          if (ad.isActive) {
            message = this.languageService.lang == 'en' ? 'Activated Successfully !' : 'تم تفعيل الإعلان بنجاح !';
          } else {
            message = this.languageService.lang == 'en' ? 'Deactivated successfully !' : 'تم إلغاء الإعلان بنجاح !';
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

  openDialog(event: any, ad: Ad) {
    let targetAttr = event.target.getBoundingClientRect();

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    if (this.languageService.lang == 'en') {
      dialogConfig.data = {
        massage: "Are you sure to delete this ad?"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width - 80 + "px"
      };
    } else {
      dialogConfig.data = {
        massage: "هل أنت متأكد من حذف هذا الإعلان"
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
          this.adService.deleteAd(ad.compoundAdId).subscribe(({ ok, result, message }) => {
            if (ok) {
              this.dataSource.data.splice(this.dataSource.data.indexOf(ad), 1);
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
    this.adFilter.pageNumber = page + 1;
    this.populateAd();
  }
}
