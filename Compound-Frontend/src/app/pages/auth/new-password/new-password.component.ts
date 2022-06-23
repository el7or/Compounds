import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LocalStorageService } from 'angular-2-local-storage';
import { UsersService } from 'src/app/services/api/login/users.service';
import { CurrentUserService } from 'src/app/services/current-user.serives';

@Component({
  selector: 'app-new-password',
  templateUrl: './new-password.component.html',
  styleUrls: ['./new-password.component.scss']
})
export class NewPasswordComponent implements OnInit {


  visibility1: boolean = false;
  visibility2: boolean = false;

  loading: boolean = false;
  form: FormGroup = new FormGroup({
    newpassword: new FormControl(null, [Validators.required]),
    confirmpassword: new FormControl(null, [Validators.required])
  });

  constructor(
    private currentUserService: CurrentUserService,
    private usersService: UsersService,
    private localStorageService: LocalStorageService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {

  }

  SetNewPaswword() {

  }
}
