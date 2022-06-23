import { IGroup } from './compound-groups.service';
import { Injectable } from '@angular/core';
import { WebApiService } from '../../webApi.service';
import { HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CompanyUnitsService {

  private controller: string = 'CompanyUnits';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getCompanyUnits(param: HttpParams) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}`, param);
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
