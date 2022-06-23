import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { CurrentUserService } from '../../current-user.serives';
import { TokenExpiresTimerService } from '../../token-expires-timer.serives';

@Injectable({
  providedIn: 'root'
})
export class LoginGuard implements CanActivate {
  constructor(
    private tokenExpiresTimerService: TokenExpiresTimerService,
    private currentUserService: CurrentUserService,
    private router: Router
  ) { }
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (this.currentUserService.currentUser && this.currentUserService.currentUser.accessToken) {
      if (!this.tokenExpiresTimerService.isTimerRunning)
        this.tokenExpiresTimerService.startTimer();
      return true;
    } else {
      this.router.navigate(['auth', 'login']);
      return false;
    }
  }
}
