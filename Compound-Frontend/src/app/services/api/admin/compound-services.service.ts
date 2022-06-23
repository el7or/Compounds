import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { PagedInput, WebApiService } from '../../webApi.service';

@Injectable({
  providedIn: 'root'
})
export class CompoundServicesService {

  private controller: string = 'Services';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getFilteredRequests(requestsFilter: ServiceRequestsFilter) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, requestsFilter);
  }

  getRequest(requestId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${requestId}`);
  }

  getServices(compoundId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/compound-services/${compoundId}`);
  }

  updateServices(compoundId: string, services: any[]) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}/compound-services/${compoundId}`, services);
  }

  updateRequestStatus(requestId: string, statusObj: {}) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}/${requestId}/status`, statusObj);
  }

  updateRequestComment(requestId: string, commentObj: {}) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}/${requestId}/comment`, commentObj);
  }

  getCompoundsServices(compounds: any) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}/compounds-services`, compounds)
  }

}

export class ServiceRequestsFilter extends PagedInput {
  compoundId?: string;
  companyId?: string;
  serviceTypeIds?: string[];
  searchText?: string;
  from?: Date;
  to?: Date;
  status?: number;
  isActive?: boolean;
  public constructor(init?: Partial<ServiceRequestsFilter>) {
    super(init);
    Object.assign(this, init);
  }
}

export class ServiceRequest {
  serviceRequestId!: string;
  compoundId!: string;
  companyId!: string;
  requestedBy!: string;
  serviceTypeArabic!: string;
  serviceTypeEnglish!: string;
  postTime!: Date;
  status!: Number;
  isActive!: boolean;
  from!: Date;
  to!: Date;
  rate!: Number;
  presenterRate!: Number;
  comment!: string;
  ownerComment!: string;
  ownerName!: string;
  ownerPhone!: string;
  requestNumber!: Number;
  unitName!: string;
  note!: string;
  record!: string;
  icon!: string;
  attachments!: string[];
  serviceSubTypesTotalCost?: number;
  serviceSubTypes?: ServiceRequestSubType[];
}

export enum ServiceStatus {
  pending,
  done,
  cancelled
}
export enum IssueStatus {
  pending,
  done,
  cancelled
}

export class CompoundServiceType {
  serviceTypeId!: string;
  serviceNameArabic!: string;
  serviceNameEnglish!: string;
  selected!: boolean;
  isFixed!: boolean;
  assignOrder!: number;
  serviceOrder!: number;
  serviceSubTypes?: ServiceSubType[];
}
export class ServiceSubType {
  serviceSubTypeId?: string;
  serviceTypeId?: string;
  compoundServiceId?: string;
  compoundId?: string;
  arabicName!: string;
  englishName!: string;
  cost!: number;
  order?: number;
  isEditing?: boolean;
}
export class ServiceRequestSubType {
  serviceRequestSubTypeId?: string;
  serviceRequestId?: string;
  serviceSubTypeId?: string;
  serviceSubTypeEnglish?: string;
  serviceSubTypeArabic?: string;
  serviceSubTypeCost?: number;
  serviceSubTypeQuantity?: number;
  order?: number;
}
