import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { PagedInput, WebApiService } from '../../webApi.service';

@Injectable({
  providedIn: 'root'
})
export class CompoundIssuesService {

  private controller: string = 'Issues';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getFilteredRequests(requestsFilter: IssueRequestsFilter) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, requestsFilter);
  }

  getRequest(requestId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${requestId}`);
  }

  getIssues(compoundId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/compound-issues/${compoundId}`);
  }

  updateIssues(compoundId: string, issues: any[]) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}/compound-issues/${compoundId}`, issues);
  }

  updateRequestStatus(requestId: string, statusObj: {}) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}/${requestId}/status`, statusObj);
  }

  updateRequestComment(requestId: string, commentObj: {}) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}/${requestId}/comment`, commentObj);
  }

  getCompoundsIssues(compounds: any) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}/compounds-issues`, compounds)
  }

}

export class IssueRequestsFilter extends PagedInput {
  compoundId?: string;
  companyId?: string;
  issueTypeIds?: string[];
  searchText?: string;
  status?: number;
  isActive?: boolean;
  public constructor(init?: Partial<IssueRequestsFilter>) {
    super(init);
    Object.assign(this, init);
  }
}

export class IssueRequest {
  issueRequestId!: string;
  compoundId!: string;
  companyId!: string;
  requestedBy!: string;
  issueTypeArabic!: string;
  issueTypeEnglish!: string;
  postTime!: Date;
  status!: Number;
  isActive!: boolean;
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
}

export enum IssueStatus {
  pending,
  done,
  cancelled
}
