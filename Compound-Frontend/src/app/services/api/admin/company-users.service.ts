import { Injectable } from '@angular/core';
import { PagedInput, WebApiService } from '../../webApi.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CompanyUsersService {

  private controller: string = 'CompanyUsers';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;
  constructor(private webApi: WebApiService) { }

  getCompanyUsers() {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}`);
  }
  postCompanyUsers(body: any) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, body);
  }
  putCompanyUsers(body: any) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}`, body);
  }
  DeleteCompanyUsers(id: number) {
    return this.webApi.delete(this.serviceSubDomain, `${this.controller}/${id}`);
  }

  searchCompanyUsers(filter:UsersFilter){
    return this.webApi.getWithFilter(this.serviceSubDomain, `${this.controller}/search`, filter);
  }

  getCompanyUser(userId:string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${userId}`);
  }

  changeUserStatus(id:string,body: any) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}/${id}/status`, body);
  }

}

export class UsersFilter extends PagedInput {
  companyId?: string;
  companyRoleId?: string;
  searchText?:string;
  public constructor(init?: Partial<UsersFilter>) {
    super(init);
    Object.assign(this, init);
  }
}

export class CompanyUser {
  companyUserId!:string;
  nameAr!:string;
  nameEn!:string;
  phone!:string;
  email!:string;
  username!:string;
  isActive?:boolean;
  isSelected?:boolean;
  currentRole?: CompanyRole;
}

export class CompoundUserInfo{
  compoundId?:string;
  selected:boolean=false;
  nameAr?:string;
  nameEn?:string;
  services:any[]=[];
  issues:any[]=[];
}

export class CompanyRole{
  companyRoleId?:string;
  companyId?:string;
  roleArabicName?:string;
  roleEnglishName?:string;
}
