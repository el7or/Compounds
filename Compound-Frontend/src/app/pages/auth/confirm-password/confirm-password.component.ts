import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LocalStorageService } from 'angular-2-local-storage';
import { UsersService } from 'src/app/services/api/login/users.service';
import { CurrentUserService } from 'src/app/services/current-user.serives';
@Component({
  selector: 'app-confirm-password',
  templateUrl: './confirm-password.component.html',
  styleUrls: ['./confirm-password.component.scss']
})
export class ConfirmPasswordComponent implements OnInit {

  visibility: boolean = false;
  loading: boolean = false;
  form: FormGroup = new FormGroup({
    email: new FormControl(null, [Validators.required, Validators.email])
  });

  constructor(
    private currentUserService: CurrentUserService,
    private usersService: UsersService,
    private localStorageService: LocalStorageService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {

  }

  verifyEmail() {

  }

}
