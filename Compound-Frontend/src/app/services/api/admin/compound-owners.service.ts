import { Iimage } from './../../webApi.service';
import { Injectable } from '@angular/core';
import { PagedInput, WebApiService } from '../../webApi.service';
import { environment } from 'src/environments/environment';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CompoundOwnersService {

  private controller: string = 'CompoundOwners';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getFilteredOwners(ownersFilter: OwnersFilter) {
    return this.webApi.getWithFilter(this.serviceSubDomain, `${this.controller}/filter`, ownersFilter);
  }
  getCompoundOwners(params: HttpParams) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}`, params);
  }
  getCompoundOwnerById(ownerId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${ownerId}`);
  }
  postCompoundOwner(body: any) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, body);
  }
  putCompoundOwner(body: any) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}`, body);
  }
  DeleteCompoundOwner(bodyId: any) {
    return this.webApi.delete(this.serviceSubDomain, `${this.controller}/${bodyId}`);
  }
}

export class OwnersFilter extends PagedInput {
  compoundId?: string;
  companyId?: string;
  name?: string;
  phone?: string;
  gender?: string;
  email?: string;
  address?: string;
  searchText?: string;
  constructor(init?: Partial<OwnersFilter>) {
    super(init);
    Object.assign(this, init);
  }
}

export class Owner {
  compoundOwnerId!: string;
  ownerRegistrationId!: string;
  name!: string;
  phone!: string;
  email!: string;
  gender!: string;
  address!: string;
  whatsAppNum!: string;
  birthDate!: string;
  subOwnersCount!: number;
  isActive!:boolean;
  creationDate!:Date;
  unitsIds!: string[];
  unitsNames!: string[];
  image!: Iimage;
  units!:any[];
  subOwners!:any[];

  fromTree?:boolean;
}
