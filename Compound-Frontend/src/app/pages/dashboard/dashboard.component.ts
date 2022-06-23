import { LanguageService } from 'src/app/services/language.serives';
import { IssueStatus, ServiceStatus } from './../../services/api/admin/compound-services.service';
import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { GoogleChartComponent, ChartType } from 'angular-google-charts'
import { Subscription } from 'rxjs';
import { ICompound, CompoundsService, CompoundDashboard, VisitsHistoryInScope, ServiceTypeStatuses, ChartScope, IssueTypeStatuses, DashboardFilter, ServiceTypeSubType, ServiceSubTypeCount } from 'src/app/services/api/admin/compounds.service';
import { VisitType } from 'src/app/services/api/admin/visits.service';
import { CompoundsStoreService } from 'src/app/services/compounds-store.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { NewsFormComponent } from '../news/news-form/news-form.component';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { CurrentUserService } from 'src/app/services/current-user.serives';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  selectedCompound: ICompound | undefined;
  info!: CompoundDashboard;
  dashboardFilter: DashboardFilter = new DashboardFilter();

  @ViewChild('chartVisits', { static: true })
  chartVisits!: GoogleChartComponent

  @ViewChild('chartServices', { static: true })
  chartServices!: GoogleChartComponent

  public get ChartType(): typeof ChartType {
    return ChartType;
  }

  columnVisits = ['Scope'];
  dataVisits?: any[];
  optionsVisits = {
    hAxis: { titleTextStyle: { color: '#333' } },
    vAxis: { minValue: 0 },
    curveType: 'function',
    legend: { position: 'bottom' },
    titleTextStyle: {
      fontSize: 20,
      bold: true
    }
  };
  selectedChartScope: ChartScope = ChartScope.Day;
  selectedChartScopeTitle: string = 'VisitsChartDaily';

  columnServices = ['Status'];
  dataServices?: any[];
  optionsServices = {
    isStacked: true,
    colors: ['orange', 'green', 'red'],
    hAxis: { titleTextStyle: { color: '#333' } },
    vAxis: { minValue: 0 },
    curveType: 'function',
    legend: { position: 'top' },
    titleTextStyle: {
      fontSize: 20,
      bold: true,
    }
  };

  columnTotalServices = ['Service', 'Percentage'];
  dataTotalServices?: any[];
  optionsTotalServices = {
    titleTextStyle: {
      fontSize: 20,
      bold: true,
    }
  };

  columnServicesRates = ['Rate', '1', '2', '3', '4', '5'];
  dataServicesRates?: any[];
  optionsServicesRates = {
    isStacked: true,
    colors: ['#ff0a0a', '#f2ce02', '#ebff0a', '#85e62c', '#209c05'],
    hAxis: { titleTextStyle: { color: '#333' } },
    vAxis: { minValue: 0 },
    curveType: 'function',
    legend: { position: 'top' },
    titleTextStyle: {
      fontSize: 20,
      bold: true,
    }
  };

  columnTotalServicesRates = ['ServiceType', 'Percentage'];
  dataTotalServicesRates?: any[];
  optionsTotalServicesRates = {
    titleTextStyle: {
      fontSize: 20,
      bold: true,
    }
  };

  columnSubTypes = ['SubType'];
  dataSubTypes?: any[];
  optionsSubTypes = {
    hAxis: {
      title: this.translateService.instant('dashboard.ServiceType'),
      titleTextStyle: {
        fontSize: 20,
        bold: true,
      }
    },
    legend: { position: 'top' },
    seriesType: 'bars',
    titleTextStyle: {
      fontSize: 20,
      bold: true,
    }
    // { 5: { type: 'line' } }
  };

  columnIssues = ['Status'];
  dataIssues?: any[];
  optionsIssues = {
    isStacked: true,
    colors: ['orange', 'green', 'red'],
    hAxis: { titleTextStyle: { color: '#333' } },
    vAxis: { minValue: 0 },
    curveType: 'function',
    legend: { position: 'top' },
    titleTextStyle: {
      fontSize: 20,
      bold: true,
    }
  };

  columnTotalIssues = ['Issue', 'Percentage'];
  dataTotalIssues?: any[];
  optionsTotalIssues = {
    titleTextStyle: {
      fontSize: 20,
      bold: true,
    }
  };

  chartWidth = window.innerWidth * 0.90;
  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this.chartWidth = window.innerWidth * 0.90;
  }

  subs = new Subscription();

  constructor(private activatedRoute: ActivatedRoute,
    private compoundsStoreService: CompoundsStoreService,
    private compoundsService: CompoundsService,
    private currentUserService: CurrentUserService,
    private snackBar: MatSnackBar,
    private loadingService: LoadingService,
    private translateService: TranslateService,
    public languageService: LanguageService,
    public rightDrawer: RightDrawer
  ) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(({ compoundId }) => {
      this.selectedCompound = this.compoundsStoreService.
        compounds?.find(i => i.compoundId == compoundId);
      this.dashboardFilter.compoundId = compoundId;
      this.dashboardFilter.serviceTypesIds = this.currentUserService.currentUser?.accessDetails?.compounds?.find(c => c.compoundId == compoundId)?.serviceTypesIds;
      this.dashboardFilter.issueTypesIds = this.currentUserService.currentUser?.accessDetails?.compounds?.find(c => c.compoundId == compoundId)?.issueTypesIds;

      this.columnVisits = ['Scope'];
      this.columnServices = ['Status'];
      this.columnTotalServices = ['Service', 'Percentage'];
      this.columnServicesRates = ['Rate', '1', '2', '3', '4', '5'];
      this.columnTotalServices = ['ServiceType', 'Percentage'];
      this.columnSubTypes = ['SubType'];
      this.columnIssues = ['Status'];
      this.columnTotalIssues = ['Issue', 'Percentage'];
      this.dataVisits = [];
      this.dataServices = [];
      this.dataTotalServices = [];
      this.dataServicesRates = [];
      this.dataTotalServicesRates = [];
      this.dataSubTypes = [];
      this.dataIssues = [];
      this.dataTotalIssues = [];
      this.populateDashboard();
    })
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  populateDashboard() {
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.compoundsService.getDashboardInfo(this.dashboardFilter).subscribe(({ ok, result, message }) => {
        if (ok) {
          this.info = result;
          /************* First Chart ****************/
          // complete columnVisits chart
          const visitTypes = Object.keys(VisitType).filter((key: any) => !isNaN(Number(VisitType[key])));
          const allVisitsHistoryCount = this.info.visitsHistoryInScope.reduce((a, b) => a + b.typeCounts.reduce((c, d) => c + d, 0), 0);
          for (let i = 0; i < visitTypes.length; i++) {
            const type = visitTypes[i];
            this.translateService.get('visits.list.' + type).subscribe((value: any) => {
              const typeVisitsCount = this.info.visitsHistoryInScope.reduce((a, b) => a + b.typeCounts[i], 0);
              const typeVisitsPercentage = parseFloat((typeVisitsCount / allVisitsHistoryCount * 100).toString()).toFixed(1);
              this.columnVisits.push(typeVisitsPercentage + '% ' + value);
            });
          }
          // populate dataVisits chart
          this.info.visitsHistoryInScope.forEach((visitsScope: VisitsHistoryInScope) => {
            let scopeArray: any[] = [visitsScope.scope];
            scopeArray.push(...visitsScope.typeCounts);
            this.dataVisits ? this.dataVisits.push(scopeArray) : this.dataVisits = [scopeArray];
          });

          /************* Second two Charts ****************/
          // complete columnServices chart
          const serviceStatuses = Object.keys(ServiceStatus).filter((key: any) => !isNaN(Number(ServiceStatus[key])));
          for (let i = 0; i < serviceStatuses.length; i++) {
            const serviceStatus = serviceStatuses[i];
            this.translateService.get('services.list.' + serviceStatus).subscribe((value: any) => {
              this.columnServices.push(value);
            });
          }
          // populate dataServices chart
          this.info.servicesTypesCounts.forEach((type: ServiceTypeStatuses) => {
            let typeColumnChartArray: any[] = this.languageService.lang == 'en' ? [type.typeEnglishName] : [type.typeArabicName];
            typeColumnChartArray.push(...type.statusCounts);
            this.dataServices ? this.dataServices.push(typeColumnChartArray) : this.dataServices = [typeColumnChartArray];
            // populate dataTotalServices chart
            let typePieChartArray: any[] = this.languageService.lang == 'en' ? [type.typeEnglishName] : [type.typeArabicName];
            const typeCount = type.statusCounts.reduce((a, b) => a + b, 0);
            typePieChartArray.push(typeCount)
            this.dataTotalServices ? this.dataTotalServices.push(typePieChartArray) : this.dataTotalServices = [typePieChartArray];
          });

          /************* Third two Charts ****************/
          // populate dataServicesRates chart
          this.info.servicesTypesCounts.forEach((type: ServiceTypeStatuses) => {
            let typeRateColumnChartArray: any[] = this.languageService.lang == 'en' ? [type.typeEnglishName] : [type.typeArabicName];
            if (type.ratesCounts.some(rate => rate > 0)) {
              typeRateColumnChartArray.push(...type.ratesCounts);
              this.dataServicesRates ? this.dataServicesRates.push(typeRateColumnChartArray) : this.dataServicesRates = [typeRateColumnChartArray];
            }
            // populate dataTotalServicesRates chart
            let typeRatePieChartArray: any[] = this.languageService.lang == 'en' ? [type.typeEnglishName] : [type.typeArabicName];
            const typeRate = type.ratesCounts.reduce((a, b) => a + b, 0);
            typeRatePieChartArray.push(typeRate)
            this.dataTotalServicesRates ? this.dataTotalServicesRates.push(typeRatePieChartArray) : this.dataTotalServicesRates = [typeRatePieChartArray];
          });

          /************* Fourth Chart ****************/
          // populate dataSubTypes chart
          this.info.servicesSubTypesCounts.forEach((type: ServiceTypeSubType) => {
            const typeName: string = this.languageService.lang == 'en' ? type.typeEnglishName : type.typeArabicName;
            this.dataSubTypes ? this.dataSubTypes.push([typeName]) : this.dataSubTypes = [[typeName]];
            type.serviceSubTypesCounts.forEach((subType: ServiceSubTypeCount) => {
              const subTypeName = this.languageService.lang == 'en' ? subType.subTypeEnglishName : subType.subTypeArabicName;
              this.columnSubTypes.push(subTypeName);
            });
          });
          this.dataSubTypes!.forEach((typeValues: any[]) => {
            for (let i = 1; i < this.columnSubTypes.length; i++) {
              typeValues.push(0)
            }
          });
          this.info.servicesSubTypesCounts.forEach((type: ServiceTypeSubType) => {
            const typeName: string = this.languageService.lang == 'en' ? type.typeEnglishName : type.typeArabicName;
            let typeValues: any[] = this.dataSubTypes?.find((t: any[]) => t.some(y => y == typeName));
            type.serviceSubTypesCounts.forEach((subType: ServiceSubTypeCount) => {
              const subTypeName = this.languageService.lang == 'en' ? subType.subTypeEnglishName : subType.subTypeArabicName;
              typeValues[this.columnSubTypes.indexOf(subTypeName)] = subType.subTypeCount;
            });
          });

          /************* Fifth two Charts ****************/
          // complete columnIssues chart
          const issueStatuses = Object.keys(IssueStatus).filter((key: any) => !isNaN(Number(IssueStatus[key])));
          for (let i = 0; i < issueStatuses.length; i++) {
            const issueStatus = issueStatuses[i];
            this.translateService.get('issues.list.' + issueStatus).subscribe((value: any) => {
              this.columnIssues.push(value);
            });
          }
          // populate dataIssues chart
          this.info.issuesTypesCounts.forEach((type: IssueTypeStatuses) => {
            let typeColumnChartArray: any[] = this.languageService.lang == 'en' ? [type.typeEnglishName] : [type.typeArabicName];
            typeColumnChartArray.push(...type.statusCounts);
            this.dataIssues ? this.dataIssues.push(typeColumnChartArray) : this.dataIssues = [typeColumnChartArray];
            // populate dataTotalIssues chart
            let typePieChartArray: any[] = this.languageService.lang == 'en' ? [type.typeEnglishName] : [type.typeArabicName];
            const typeCount = type.statusCounts.reduce((a, b) => a + b, 0);
            typePieChartArray.push(typeCount)
            this.dataTotalIssues ? this.dataTotalIssues.push(typePieChartArray) : this.dataTotalIssues = [typePieChartArray];
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

  onSelectChartScope(selectedValue: ChartScope) {
    this.loadingService.itIsLoading = true;
    this.dataVisits = [];
    this.columnVisits = ['Scope'];
    this.dashboardFilter.chartScope = selectedValue;
    this.subs.add(
      this.compoundsService.getDashboardInfo(this.dashboardFilter).subscribe(({ ok, result, message }) => {
        if (ok) {
          this.info.visitsHistoryInScope = result;
          // complete columnVisits chart
          const visitTypes = Object.keys(VisitType).filter((key: any) => !isNaN(Number(VisitType[key])));
          const allVisitsHistoryCount = this.info.visitsHistoryInScope.reduce((a, b) => a + b.typeCounts.reduce((c, d) => c + d, 0), 0);
          for (let i = 0; i < visitTypes.length; i++) {
            const type = visitTypes[i];
            this.translateService.get('visits.list.' + type).subscribe((value: any) => {
              const typeVisitsCount = this.info.visitsHistoryInScope.reduce((a, b) => a + b.typeCounts[i], 0);
              const typeVisitsPercentage = parseFloat((typeVisitsCount / allVisitsHistoryCount * 100).toString()).toFixed(1);
              this.columnVisits.push(typeVisitsPercentage + '% ' + value);
            });
          }
          // populate dataVisits chart
          this.info.visitsHistoryInScope.forEach((visitsScope: VisitsHistoryInScope) => {
            let scopeArray: any[] = [visitsScope.scope];
            scopeArray.push(...visitsScope.typeCounts);
            this.dataVisits ? this.dataVisits.push(scopeArray) : this.dataVisits = [scopeArray];
          });
          switch (selectedValue) {
            case ChartScope.Year:
              this.selectedChartScopeTitle = 'VisitsChartYearly';
              break;
            case ChartScope.Month:
              this.selectedChartScopeTitle = 'VisitsChartMonthly';
              break;
            case ChartScope.Day:
              this.selectedChartScopeTitle = 'VisitsChartDaily';
              break;
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

  onOpenNewsDetails(compoundNewsId: string | null = null) {
    this.subs.add(this.rightDrawer.open(NewsFormComponent, compoundNewsId).subscribe((arg: any) => {
      this.ngOnInit();
    }));
  }
}
