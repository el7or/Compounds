import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { WebApiService } from '../../webApi.service';

@Injectable({
  providedIn: 'root'
})
export class CompanyRoleActionsService {
  private controller: string = 'CompanyRoleActions';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  updatePagesActionsInRoles(body:UpdatePagesActionsInRoles) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}/update-list`, body);
  }
}

export class UpdatePagesActionsInRoles {
  pageActionsInRoles!: CompanyRoleAction[];
}

export class CompanyRoleAction {
  companyRoleId!: string;
  systemPageActionId!: string;
}
