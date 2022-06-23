import { Injectable } from '@angular/core';
import { WebApiService } from '../../webApi.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GatesService {

  private controller: string = 'Gates';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getGates(compoundId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/compound/${compoundId}`);
  }

  getAllGates() {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/all`);
  }

  getGate(gateId:string){
    return this.webApi.get(this.serviceSubDomain,`${this.controller}/${gateId}`);
  }

  postGates(body: any) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, body);
  }
  putGates(body: any) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}`, body);
  }

  DeleteGates(id: string,compoundId: string) {
    return this.webApi.delete(this.serviceSubDomain, `${this.controller}/${id}?compoundId=${compoundId}`);
  }
}
export interface IGate {
  gateId: String;
  gateName: String;
  compoundIds: [];
  entryType : number;

}
// enum GateEntryType {
//   t1=1,
//   t2,
//   t3
// };
