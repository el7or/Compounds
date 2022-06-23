import { IGroup } from './compound-groups.service';
import { Injectable } from '@angular/core';
import { WebApiService } from '../../webApi.service';
import { HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UnitsService {

  private controller: string = 'Units';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getUnitById(Id: any) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${Id}`);
  }
  DeleteUnits(Id: any) {
    return this.webApi.delete(this.serviceSubDomain, `${this.controller}/${Id}`);
  }
  getOwnersByUnitId(Id: any) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${Id}/owners`);
  }
  getSubOwnersByUnitId(params: { unitId: string, mainOwnerRegistrationId: string }) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/sub-owners`, new HttpParams({ fromObject: params }));
  }
  postUnits(body: any) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, body);
  }
  putUnits(body: any) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}`, body);
  }
  unitsFilter(body: { pageNumber: number, pageSize: number, text: string, compoundId: string }) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}/filter`, body);
  }
  importUnitsWithOwners(body: IAddUnitsOwners) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}/import-excel`, body);
  }
}

export interface IUnit {
  compoundUnitId: string;
  name: string;
  compoundGroupId: string;
  compoundUnitTypeId: string;
  compoundGroup: IGroup;
  ownerUnits: [];
}

export interface IAddUnitsOwners {
  compoundId: string;
  unitsOwners: IUnitOwner[];
  connectionId:string;
}

export interface IUnitOwner {
  group: string;
  unit: string;
  unitType: string;
  ownerName: string;
  mobile: string;
  email: string;
}
