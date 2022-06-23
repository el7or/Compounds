import { CurrentUserService } from 'src/app/services/current-user.serives';
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import Swal from 'sweetalert2';
import { SystemPageActions } from '../../system-page-actions.enum';

@Injectable({
  providedIn: 'root'
})
export class RoleActionGuard implements CanActivate {

  constructor(public currentUserService: CurrentUserService, public router: Router) { }

  canActivate(route: ActivatedRouteSnapshot): boolean {
    let isRoleActionExist: boolean = false;
    if (route.data?.roleActions) {
      route.data.roleActions.forEach((roleAction: SystemPageActions) => {
        isRoleActionExist = this.currentUserService.checkRolePageAction(roleAction)!;
        if (isRoleActionExist) return;
      });
    }
    if (!isRoleActionExist) {
      Swal.fire({
        title: 'ممنوع !',
        text: 'ليس لديك صلاحية الدخول إلى هذه الصفحة',
        icon: 'error',
        confirmButtonText: 'حسناً',
      }).then((result: any) => {
        if (result.value) {
          this.router.navigateByUrl('/');
        }
      });
    }
    return isRoleActionExist;
  }
}
