import { Injectable } from '@angular/core';
import { WebApiService } from '../../webApi.service';
import { TreeNodeType } from 'src/app/pages/units/compound-tree/tree-data-map.service';
import { environment } from 'src/environments/environment';
import { IUnit } from './units.service';

@Injectable({
  providedIn: 'root'
})
export class CompoundGroupsService {

  private controller: string = 'CompoundGroups';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getCompoundGroups() {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}`);
  }
  postCompoundGroups(body: any) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, body);
  }
  putCompoundGroups(body: any) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}`, body);
  }
  DeleteCompoundGroups(bodyId: any) {
    return this.webApi.delete(this.serviceSubDomain, `${this.controller}/${bodyId}`);
  }

  getUnitsCompoundGroups(bodyId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${bodyId}/units`);
  }
  getSubGroupsCompoundGroups(bodyId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${bodyId}/subGroups`);
  }
}
export interface IGroup {
  compoundGroupId: string;
  compoundId: string;
  companyId: string;
  compoundUnitId: string;
  compoundUnitTypeId: string;
  creationDate: string;
  Ar: string;
  nameEn: string;
  nameAr: string;
  name: string;
  parentGroupId: string;
  subGroupsCount: number;
  unitsCount: number;
  level: number;
  isLoading: boolean;
  treeNodeType: TreeNodeType;
  ownersCount: number;
  subOwnersCount: number;
  forEdit: boolean;

  compoundOwnerId: string;
  ownerRegistrationId: string;
  subOwners: number;
  email?:string;
  phone?:string;
  userType?:any;

  compoundUnits?:IUnit[];
}
