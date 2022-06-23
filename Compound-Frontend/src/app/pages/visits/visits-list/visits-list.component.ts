import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { VisitViewComponent } from './../visit-view/visit-view.component';
import { VisitType } from './../../../services/api/admin/visits.service';
import { ChangeDetectorRef, Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { ICompound } from 'src/app/services/api/admin/compounds.service';
import { Visit, VisitsFilter, VisitsService, VisitStatus } from 'src/app/services/api/admin/visits.service';
import { CompoundsStoreService } from 'src/app/services/compounds-store.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { FormBuilder, FormGroup } from '@angular/forms';
import { map, startWith } from 'rxjs/operators';
import { MatOptionSelectionChange } from '@angular/material/core';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { ElementRef } from '@angular/core';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';
import { CurrentUserService } from 'src/app/services/current-user.serives';

@Component({
  selector: 'app-visits-list',
  templateUrl: './visits-list.component.html',
  styleUrls: ['./visits-list.component.scss']
})
export class VisitsListComponent implements OnInit, OnDestroy {
  displayedColumns = [
    'position',
    'ownerName',
    'visitorName',
    'unitName',
    'gateName',
    'date',
    'type',
    'status',
    'actions',
  ];
  selectedCompound: ICompound | undefined
  form: FormGroup = Object.create(null);

  compoundOwners: any[] = [];
  compoundUnits: any[] = [];
  compoundGates: any[] = [];
  filteredOwners!: Observable<any[]>;
  filteredUnits!: Observable<string[]>;
  allUnits!: string[];
  selectedUnits!: string[];
  separatorKeysCodes: number[] = [ENTER, COMMA];
  @ViewChild('unitInput') unitInput!: ElementRef<HTMLInputElement>;

  visitsFilter = new VisitsFilter({
    pageNumber: 1,
    pageSize: 5,
    sortBy: 'date',
    isSortAscending: false,
    withFilterLists: true
  });
  totalItems!: number;
  dataSource!: MatTableDataSource<Visit>;
  @ViewChild(MatSort) sort!: MatSort;
  loading: boolean = false;
  statusParam!: any;

  subs = new Subscription();

  private _filterUnit(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.allUnits.filter(unit => unit.toLowerCase().includes(filterValue));
  }

  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }

  constructor(private activatedRoute: ActivatedRoute,
    private compoundsStoreService: CompoundsStoreService,
    private visitsService: VisitsService,
    private snackBar: MatSnackBar,
    private loadingService: LoadingService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef,
    public rightDrawer: RightDrawer,
    public currentUserService: CurrentUserService,
    public languageService: LanguageService) {
    this.statusParam = this.activatedRoute.snapshot.queryParamMap.get('status');
    if (this.statusParam)
      this.visitsFilter.status = this.statusParam;
    else
      this.statusParam = "";
  }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(({ compoundId }) => {
      this.selectedCompound = this.compoundsStoreService.
        compounds?.find(i => i.compoundId == compoundId);
      this.visitsFilter.compoundId = compoundId;
      this.populateVisits();
    });

    this.form = this.fb.group({
      ownerRegistrationId: [null],
      compoundUnitsIds: [null],
      gateId: [null],
      date: [null],
      dateFrom: [null],
      dateTo: [null],
      status: [this.statusParam]
    });
  }

  ngAfterViewInit() {
    this.cdr.detectChanges();
    this.subs.add(
      this.sort.sortChange.subscribe(
        (result: any) => {
          this.visitsFilter.sortBy = result.active;
          this.visitsFilter.isSortAscending = result.direction == 'asc';
          this.populateVisits();
        }
      )
    );
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  populateVisits() {
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.visitsService.getFilteredVisits(this.visitsFilter).subscribe(({ ok, result, message }) => {
        if (ok) {
          result.result.forEach((visit: Visit) => {
            visit.status = VisitStatus[visit.status];
            visit.type = VisitType[visit.type];
          });
          this.dataSource = new MatTableDataSource(result.result);
          this.totalItems = result.totalCount;
          if (this.visitsFilter.withFilterLists) {
            this.compoundOwners = result.compoundOwners;
            this.compoundUnits = result.compoundUnits;
            this.compoundGates = result.compoundGates;
            this.allUnits = this.compoundUnits.map(unit => { return unit.name });
            this.filteredOwners = this.form.controls['ownerRegistrationId'].valueChanges.pipe(
              startWith(''), map(value => {
                return this.compoundOwners.filter(owner => owner.name?.toLowerCase().includes(value?.toLowerCase()))
              }));
            this.filteredUnits = this.form.controls['compoundUnitsIds'].valueChanges.pipe(
              startWith(null),
              map((unit: string | null) => unit ? this._filterUnit(unit) : this.allUnits.slice()));
            this.visitsFilter.withFilterLists = false;
          }
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

  onExportExcel() {
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.visitsService.exportExcelVisitsList(this.visitsFilter).subscribe(({ ok, result, message }) => {
        if (ok) {
          var link = document.createElement('a');
          link.href = result.substring(0, result.length - 1).substr(1);
          link.click();
        } else {
          this.snackBar.open(message, 'ERROR', {
            duration: 3000,
            horizontalPosition: 'start',
            panelClass: ['bg-danger']
          })
        }
        this.loadingService.itIsLoading = false;
      }));
  }


  selectOwner(event: MatOptionSelectionChange) {
    this.visitsFilter.ownerRegistrationId = event.source.id;
  }

  selectUnit(event: MatAutocompleteSelectedEvent): void {
    this.selectedUnits ? this.selectedUnits!.push(event.option.viewValue) : this.selectedUnits = [event.option.viewValue];
    const index = this.allUnits!.indexOf(event.option.viewValue);
    if (index >= 0) {
      this.allUnits!.splice(index, 1);
    }
    this.unitInput.nativeElement.value = '';
    this.form.controls['compoundUnitsIds'].setValue(null);
  }

  removeUnit(unit: string): void {
    const index = this.selectedUnits!.indexOf(unit);
    if (index >= 0) {
      this.selectedUnits!.splice(index, 1);
    }
    this.allUnits ? this.allUnits!.push(unit) : this.allUnits = [unit];
  }

  onFilter() {
    if (this.selectedUnits)
      this.visitsFilter.compoundUnitsIds = this.selectedUnits!.map(unit => {
        return this.compoundUnits.find(u => u.name === unit).compoundUnitId;
      });
    this.visitsFilter.gateId = this.form.value.gateId;
    this.visitsFilter.status = this.form.value.status;
    if (this.form.value.date)
      this.visitsFilter.date = new Date(this.form.value.date).toDateString();
    if (this.form.value.dateFrom)
      this.visitsFilter.dateFrom = this.mergDateTime(this.form.value.date, this.form.value.dateFrom);
    if (this.form.value.dateTo)
      this.visitsFilter.dateTo = this.mergDateTime(this.form.value.date, this.form.value.dateTo);
    this.visitsFilter.pageNumber = 1;
    this.totalItems = 0;
    this.populateVisits();
  }

  onReset() {
    this.form.reset();
    this.form.controls["status"].patchValue("");
    this.visitsFilter.ownerRegistrationId = null;
    this.visitsFilter.compoundUnitsIds = null;
    this.visitsFilter.gateId = null;
    this.visitsFilter.date = null;
    this.visitsFilter.dateFrom = null;
    this.visitsFilter.dateTo = null;
    this.visitsFilter.status = null;
    this.visitsFilter.pageNumber = 1;
    this.totalItems = 0;
    this.populateVisits();
  }

  onOpenDetails(visit: Visit) {
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.visitsService.getVisit(visit.visitRequestId).subscribe(({ ok, result, message }) => {
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
  }

  onPageChange(page: number) {
    this.loadingService.itIsLoading = true;
    this.visitsFilter.pageNumber = page + 1;
    this.populateVisits();
  }

  mergDateTime(date: Date, time: Date) {
    // =====> to merge date with time
    let dateTime = new Date(
      date.getFullYear(),
      date.getMonth(),
      date.getDate(),
      time.getHours(),
      time.getMinutes()
    );
    // =====> to clear time from timezone:
    // dateTime = new Date(dateTime);
    // const timeZoneDifference = (dateTime.getTimezoneOffset() / 60) * -1;
    // dateTime.setTime(dateTime.getTime() + timeZoneDifference * 60 * 60 * 1000);
    return dateTime.toISOString();
  }

  fromUtcToTimeZone(date: Date) {
    date = new Date(date);
    const timeZoneDifference = (date.getTimezoneOffset() / 60) * -1;
    return new Date(date.setHours(date.getHours() + timeZoneDifference));
  }
}
