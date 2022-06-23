import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Iimage, PagedInput, WebApiService } from '../../webApi.service';

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {

  private controller: string = 'CompoundNotifications';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getFilteredNotifications(notificationFilter: NotificationFilter) {
    return this.webApi.getWithFilter(this.serviceSubDomain, `${this.controller}`, notificationFilter);
  }
  getNotification(notificationId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${notificationId}`);
  }
  postNotification(body: Notification) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, body);
  }
  putNotification(body: Notification) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}`, body);
  }
  deleteNotification(notificationId: string) {
    return this.webApi.delete(this.serviceSubDomain, `${this.controller}/${notificationId}`);
  }
}

export class NotificationFilter extends PagedInput {
  compoundId?: string;
  ownerRegistrationId?: string;
  searchText?:string;
  public constructor(init?: Partial<NotificationFilter>) {
    super(init);
    Object.assign(this, init);
  }
}

export class Notification {
  compoundNotificationId!: string;
  compoundId!: string;
  companyId!: string;
  englishTitle!: string;
  arabicTitle!: string;
  englishMessage!: string;
  arabicMessage!: string;
  isOwnerOnly?: boolean;
  toGroupsIds?:string[];
  toUnitsIds?:string[];
  images?: Iimage[];
}
