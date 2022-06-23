import { NgModule } from '@angular/core';
import { LoginComponent } from './login/login.component';
import { RouterModule } from '@angular/router';
import { SharedModule } from 'src/app/shared/shared.module';
import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { UsersService } from 'src/app/services/api/login/users.service';
import { CurrentUserService } from 'src/app/services/current-user.serives';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { NewPasswordComponent } from './new-password/new-password.component';
import { ConfirmPasswordComponent } from './confirm-password/confirm-password.component';

@NgModule({
  declarations: [
    LoginComponent,
    ResetPasswordComponent,
    NewPasswordComponent,
    ConfirmPasswordComponent
  ],
  imports: [
    SharedModule,
    FormLayoutModule,
    RouterModule.forChild([
      { path: 'login', component: LoginComponent },
      { path: 'reset-password', component: ResetPasswordComponent },
      { path: 'new-password', component: NewPasswordComponent },
      { path: 'confirm-password', component: ConfirmPasswordComponent },
      { path: '**', pathMatch: 'full', redirectTo: "login" }
    ]),
  ],
  providers: [UsersService, CurrentUserService]
})
export class LoginModule { }
