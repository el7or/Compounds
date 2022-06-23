import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { PagedInput, WebApiService } from '../../webApi.service';

@Injectable({
  providedIn: 'root'
})
export class VisitsService {
  private controllerVisitRequest: string = 'VisitRequests';
  private controllerVisitHistory: string = 'VisitHistories';
  private serviceSubDomain: string = environment.serviceSubDomain_visits;

  constructor(private webApi: WebApiService) { }

  getFilteredVisits(visitsFilter: VisitsFilter) {
    return this.webApi.getWithFilter(this.serviceSubDomain, `${this.controllerVisitHistory}`, visitsFilter);
  }
  getVisit(visitId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controllerVisitRequest}/${visitId}`);
  }
  confirmVisit(body: VisitRequestApprove) {
    return this.webApi.post(this.serviceSubDomain, `${this.controllerVisitRequest}/approve`, body);
  }
  exportExcelVisitsList(visitsFilter: VisitsFilter) {
    return this.webApi.post(this.serviceSubDomain, `${this.controllerVisitHistory}/excel`, visitsFilter);
  }
}

export class VisitsFilter extends PagedInput {
  compoundId?: string;
  ownerRegistrationId?: string | null;
  compoundUnitsIds?: string[] | null;
  gateId?: string | null;
  date?: string | null;
  dateFrom?: string | null;
  dateTo?: string | null;
  status?: string | null;
  withFilterLists?: boolean;
  constructor(init?: Partial<VisitsFilter>) {
    super(init);
    Object.assign(this, init);
  }
}

export class Visit {
  visitTransactionHistoryId!: string;
  visitRequestId!: string;
  ownerRegistrationId!: string;
  ownerName!: string;
  date?: Date;
  gateName!: string;
  unitName!: string;
  type!: any;
  status!: any;
}

export class VisitDetails {
  visitRequestId!: string;
  ownerRegistrationId!: string;
  compoundUnitId!: string;
  code!: string;
  ownerName!: string;
  userType!: any;
  unitName!: string;
  visitorName!: string;
  type!: any;
  status!: any;
  dateFrom!: string;
  dateTo!: string;
  groupNo!: number;
  days!: any[];
  carNo!: string;
  details!: string;
  files!: string[];
  isCanceled!: boolean;
  isConfirmed!: boolean;
  isConsumed!: boolean;
  qrCode!: string;
  owner!: VisitOwner;
  attachments!:VisitAttachment[];
}

export enum VisitType {
  None,
  Once,
  Periodic,
  Labor,
  Group,
  Taxi,
  Delivery
}

export enum VisitStatus {
  Consumed = 1,
  Pending,
  Confirmed,
  NotConfirmed,
  Canceled,
  Expired
}

export enum VisitDay {
  Saturday = 1,
  Sunday,
  Monday,
  Tuesday,
  Wednesday,
  Thursday,
  Friday
}

export class VisitOwner {
  compoundOwnerId!: string;
  name!: string;
  phone!: string;
  email!: string;
  units!: string[];
}

export class VisitRequestApprove {
  isApproved!: boolean
  id!: string
  attachments?: VisitAttachment[];
}
export class VisitAttachment {
  sizeInBytes?:number;
  fileName?:string;
  path?:string;
  fileBase64?:string;
}
