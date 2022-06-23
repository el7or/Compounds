import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { PagedInput, WebApiService } from '../../webApi.service';

@Injectable({
  providedIn: 'root'
})
export class CompoundsService {

  private controller: string = 'Compounds';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getCompounds() {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}`);
  }
  getAllGroupsByCompoundId(compoundId: any) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${compoundId}/all-groups`);
  }
  getParentGroupsByCompoundId(compoundId: any) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${compoundId}/parent-groups`);
  }
  getGatesByCompoundId(compoundId: any) {
    return this.webApi.get(this.serviceSubDomain, `Gates/${compoundId}`);
  }
  getCompoundByCompoundId(compoundId: any) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${compoundId}`);
  }
  postCompounds(Compounds: any) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, Compounds);
  }
  putCompounds(compounds: any) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}`, compounds);
  }
  DeleteCompounds(compoundId: any) {
    return this.webApi.delete(this.serviceSubDomain, `${this.controller}/${compoundId}`);
  }
  getDashboardInfo(body: DashboardFilter) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}/dashboard`, body);
  }
  getAllPendings(body: AllPendingsFilter) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}/all-pendings`, body);
  }
  getAllPendingsCount(body: AllPendingsFilter) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}/all-pendings-count`, body);
  }
}

export interface ICompound {
  address: string;
  companyId: string;
  compoundId: string;
  creationDate: Date;
  email: string;
  image: string;
  location: any;
  mobile: string;
  nameAr: string;
  nameEn: string;
  phone: string;
  emergencyPhone: string;
  ownersCount?: number;
  unitsCount?: number;
  servicesCount?: number;
}

export class CompoundDashboard {
  allUsersCount!: number;
  pendingUsersCount!: number;
  allVisitsCount!: number;
  pendingVisitsCount!: number;
  allServicesCount!: number;
  allIssuesCount!: number;
  pendingServicesCount!: number;
  pendingIssuesCount!: number;
  visitsHistoryInScope!: VisitsHistoryInScope[];
  servicesTypesCounts!: ServiceTypeStatuses[];
  servicesSubTypesCounts!: ServiceTypeSubType[];
  issuesTypesCounts!: IssueTypeStatuses[];
}
export class VisitsHistoryInScope {
  scope!: string;
  typeCounts!: number[];
}
export class ServiceTypeStatuses {
  typeEnglishName!: string;
  typeArabicName!: string;
  statusCounts!: number[];
  ratesCounts!: number[];
}
export class ServiceTypeSubType {
  typeEnglishName!: string;
  typeArabicName!: string;
  serviceSubTypesCounts!: ServiceSubTypeCount[];
}
export class ServiceSubTypeCount {
  subTypeEnglishName!: string;
  subTypeArabicName!: string;
  subTypeCount!: number;
}
export class IssueTypeStatuses {
  typeEnglishName!: string;
  typeArabicName!: string;
  statusCounts!: number[];
}
export enum ChartScope {
  Year = 1,
  Month = 2,
  Day = 3
}
export class DashboardFilter {
  compoundId!: string;
  chartScope!: ChartScope;
  serviceTypesIds?: string[];
  issueTypesIds?: string[];
  public constructor(init?: Partial<DashboardFilter>) {
    Object.assign(this, init);
  }
}

export class AllPendingsFilter extends PagedInput {
  compoundsIds!: string[];
  isShowUsers?: boolean;
  isShowVisits?: boolean;
  isShowServices?: boolean;
  isShowIssues?: boolean;
  public constructor(init?: Partial<AllPendingsFilter>) {
    super(init);
    Object.assign(this, init);
  }
}
export class PendingListItem {
  id!: string;
  pendingType!: PendingType;
  compoundId!: string;
  compoundNameEn!: string;
  compoundNameAr!: string;
  createdDate!: Date;
  createdByName!: string;
  routeLink?: string[];
  queryParams?: any;
  icon?: string;
  ownerRegistrationPhone?: string;
}
export enum PendingType {
  User = 1,
  Visit = 2,
  Service = 3,
  Issue = 4
}
