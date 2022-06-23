import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { WebApiService } from '../../webApi.service';

@Injectable({
  providedIn: 'root'
})
export class CompanyUserRolesService {
  private controller: string = 'CompanyUserRoles';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  updateUsersRoles(body:UpdateUserRoles) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}/update-list`, body);
  }
}

export class UpdateUserRoles {
  usersRoles!: CompanyUserRole[];
}

export class CompanyUserRole {
  companyUserId!: string;
  companyRoleId!: string;
}
