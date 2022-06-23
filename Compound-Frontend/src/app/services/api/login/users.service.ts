import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { WebApiService } from '../../webApi.service';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  private controller: string = 'Users';
  private serviceSubDomain: string = environment.serviceSubDomain_login;

  constructor(private webApi: WebApiService) { }

  authenticate(user: { username: string, password: string }) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}/authenticate`, user);
  }
  refreshToken(token: string) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}/refresh-token`, { refreshToken: token });
  }
  revokeToken(token: string) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}/revoke-token`, token);
  }
}
