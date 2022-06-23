import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { PagedInput, WebApiService } from '../../webApi.service';

@Injectable({
  providedIn: 'root'
})
export class CompoundReportsService {

  private controller: string = 'CompoundReports';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getCompoundsReports(compounds:any){
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`,compounds)
  }

}
