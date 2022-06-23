import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { PagedInput, WebApiService } from '../../webApi.service';

@Injectable({
  providedIn: 'root'
})
export class SystemPagesService {
  private controller: string = 'SystemPages';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getPagesWithActionsByRole(roleId:string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/role/${roleId}`);
  }
}

export class SystemPage {
  systemPageId!: string;
  pageArabicName!: string;
  pageEnglishName!: string;
  systemPageActions!: SystemPageAction[];
}

export class SystemPageAction {
  systemPageActionId!: string;
  actionArabicName!: string;
  actionEnglishName!: string;
  actionUniqueName!: string;
  isSelected?: boolean;
}
