import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { PagedInput, WebApiService } from '../../webApi.service';

@Injectable({
  providedIn: 'root'
})
export class RolesService {
  private controller: string = 'CompanyRoles';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getRolesByName(rolesFilter: RolesFilter) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}/filter`, rolesFilter);
  }
  getRolesByCompanyId(rolesFilter: RolesFilter) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}/${rolesFilter.companyId}/roles`, rolesFilter);
  }
  getRoleById(roleId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${roleId}`);
  }
  postRole(body: Role) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, body);
  }
  putRole(body: Role) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}`, body);
  }
  deleteRole(roleId: string) {
    return this.webApi.delete(this.serviceSubDomain, `${this.controller}/${roleId}`);
  }
}

export class RolesFilter extends PagedInput {
  companyId?: string;
  text?:string;
  public constructor(init?: Partial<RolesFilter>) {
    super(init);
    Object.assign(this, init);
  }
}

export class Role {
  companyRoleId!: string;
  companyId!: string;
  roleEnglishName!: string;
  roleArabicName!: string;
}
