import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { CurrentUserService } from './current-user.serives';
import * as moment from 'moment';
import { UsersService } from './api/login/users.service';

@Injectable({
  providedIn: 'root'
})
export class TokenExpiresTimerService {
  private timer: any;

  constructor(
    private currentUserService: CurrentUserService,
    private router: Router,
    private usersService: UsersService
  ) { }

  startTimer() {
    let { accessTokenExpiresIn, serverTimeMs } = this.currentUserService.currentUser
    this.timer = setInterval(() => {
      serverTimeMs++;
      this.currentUserService.currentUser = { ...this.currentUserService.currentUser, serverTimeMs };
      if (serverTimeMs >= accessTokenExpiresIn) this.refreshToken();
    }, 800)
  }

  refreshToken() {
    this.clearTimer();
    this.usersService.refreshToken(this.currentUserService.currentUser.refreshToken).subscribe(({ ok, result }) => {
      if (ok) {
        this.currentUserService.currentUser = result;
        this.startTimer();
      } else {
        this.router.navigate(['/auth/login']);
      }
    },
      () => this.router.navigate(['/auth/login'])
    )
  }

  get isTimerRunning(): boolean {
    return this.timer as boolean;
  }

  clearTimer() {
    if (this.isTimerRunning) {
      clearInterval(this.timer);
      this.timer = undefined;
    }
  }
}
