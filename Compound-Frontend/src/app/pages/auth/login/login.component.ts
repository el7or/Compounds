import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { UsersService } from 'src/app/services/api/login/users.service';
import { CurrentUserService } from 'src/app/services/current-user.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { TokenExpiresTimerService } from 'src/app/services/token-expires-timer.serives';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  visibility: boolean = false;
  loading: boolean = false;
  currentLang:string='' ;
  form: FormGroup = new FormGroup({
    username: new FormControl(null, [Validators.required, Validators.email]),
    password: new FormControl(null, [Validators.required])
  });

  constructor(
    private currentUserService: CurrentUserService,
    private usersService: UsersService,
    private snackBar: MatSnackBar,
    private router: Router,
    private tokenExpiresTimerService: TokenExpiresTimerService,
    public languageService: LanguageService) {
      if(this.languageService.lang=='en')
      {
        this.currentLang='العربية';
      }else{
        this.currentLang='English';
      }}

      changeLang(){
        if(this.languageService.lang=='en')
        {
          this.languageService.selectLang('ar');
        }else{
          this.languageService.selectLang('en');
        }}
  ngOnInit(): void {
    this.currentUserService.logout();
    this.tokenExpiresTimerService.clearTimer();
  }

  login() {
    this.loading = true;
    this.usersService.authenticate(this.form.value).subscribe(({ ok, result, message }) => {
      this.loading = false;
      if (ok) {
        this.currentUserService.currentUser = result;
        this.router.navigate(['/compounds']);
      } else {
        this.snackBar.open(message, 'OK', {
          duration: 3000,
          horizontalPosition: 'start',
          panelClass: ['bg-danger']
        });
      }
    })
  }
}
