import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { LocalStorageService } from 'angular-2-local-storage';
import { SystemPageActions } from './system-page-actions.enum';

@Injectable({
  providedIn: 'root'
})
export class CurrentUserService {
  constructor(private localStorageService: LocalStorageService, private router: Router) { }
  get currentUser(): ICurrentUser {
    return this.localStorageService.get('~CurrentUser~') as ICurrentUser;
  };
  set currentUser(currentUser: ICurrentUser) {
    this.localStorageService.set('~CurrentUser~', currentUser);
  }
  logout() {
    this.localStorageService.remove('~CurrentUser~');
  }

  checkRolePageAction(pageAction: SystemPageActions) {
    return this.currentUser?.accessDetails?.userActions?.indexOf(pageAction)! > -1;
  }
}
interface ICurrentUser {
  accessDetails: AccessDetails;
  accessToken: string;
  accessTokenExpiresIn: number;
  companyId: string;
  id: string;
  refreshToken: string;
  refreshTokenExpiresIn: number;
  serverTimeMs: number;
  userType: number;
}
interface AccessDetails {
  compounds?: CompoundsIds[];
  rolesIds?: string[];
  userActions?: string[];
}
interface CompoundsIds {
  compoundId?: string;
  serviceTypesIds?: string[];
  issueTypesIds?: string[];
}
