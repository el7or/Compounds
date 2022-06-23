import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subscription } from 'rxjs';
import { CompaniesService } from 'src/app/services/api/admin/companies.service';
import { CompanyUsersService, CompoundUserInfo } from 'src/app/services/api/admin/company-users.service';
import { CompoundReportsService } from 'src/app/services/api/admin/compound-reports.service';
import { CompoundServicesService } from 'src/app/services/api/admin/compound-services.service';
import { CurrentUserService } from 'src/app/services/current-user.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';

@Component({
  selector: 'company-user',
  templateUrl: './company.user.component.html',
  styleUrls: ['./company.user.component.scss']
})
export class CompanyUserComponent implements OnInit, OnDestroy {

  userId?: string | null;
  form: FormGroup = Object.create(null);
  loading: boolean = false;
  visibility1: boolean = false;
  visibility2: boolean = false;
  compounds: CompoundUserInfo[] = [];
  subs = new Subscription();

  
  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }


  constructor(public rightDrawer: RightDrawer,
    private fb: FormBuilder,
    private loadingService: LoadingService,
    public currentUserService: CurrentUserService,
    private companyService: CompaniesService,
    public languageService: LanguageService,
    private servicesService: CompoundServicesService,
    private reportsService: CompoundReportsService,
    private usersService: CompanyUsersService,
    private snackBar: MatSnackBar) {
    this.userId = this.rightDrawer.parentData;
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      userId: [null],
      nameAr: [null, Validators.compose([Validators.required])],
      nameEn: [null, Validators.compose([Validators.required])],
      phone: [null, Validators.compose([Validators.required])],
      email: [null, Validators.compose([Validators.required])],
      username: [null, Validators.compose([Validators.required])],
      password: [null, Validators.compose([Validators.required])],
      confirmPassword: [null, Validators.compose([Validators.required])],
      userImage: [null, Validators.compose([Validators.required])]
    });
    // get company compounds
    this.companyService.getCompanyCompounds(this.currentUserService.currentUser.companyId)
      .subscribe(({ ok, result }) => {
        if (ok) {
          this.compounds = result;
          // edit user
          if (this.userId) {
            this.subs.add(
              this.usersService.getCompanyUser(this.userId).subscribe(({ ok, result, message }) => {
                if (ok) {
                  this.form.patchValue({
                    companyUserId: this.userId,
                    nameAr: result.nameAr,
                    nameEn: result.nameEn,
                    phone: result.phone,
                    email: result.email,
                    username: result.username,
                    password: result.password,
                    confirmPassword: result.password,
                    userImage: result.userImage
                  });
                  this.setCompoundChecked(result);
                } else {
                  this.snackBar.open(message, 'ERROR', {
                    duration: 3000,
                    horizontalPosition: 'start',
                    panelClass: ['bg-danger']
                  })
                }
              })
            );
          }
        }
      });
  }

  setCompoundChecked(userData: any) {
    if (this.compounds) {
      this.compounds.forEach(compound => {
        const retrieved = userData.companyUserCompounds.find((x: { compoundId: string | undefined; }) => x.compoundId == compound.compoundId);
        if (retrieved) {
          compound.selected = true;
          if (compound.services) {
            compound.services.forEach(ser => {
              const servExist = retrieved.services.find((s: any) => s == ser.serviceTypeId);
              if (servExist) ser.selected = true;
            });
          }
          if (compound.issues) {
            compound.issues.forEach(issue => {
              const issueExist = retrieved.issues.find((s: any) => s == issue.issueTypeId);
              if (issueExist) issue.selected = true;
            });
          }
        }
      });
    }
  }

  ngOnDestroy() {

  }

  onSubmit() {
    this.loading = true;
    this.loadingService.itIsLoading = true;
    let userObj = this.form.value;
    userObj.companyId = this.currentUserService.currentUser.companyId;
    userObj.userType = 1;
    userObj.compounds = this.compounds.filter(x => x.selected).map(function (elem) {
      return {
        compoundId: elem.compoundId,
        services: elem.services.filter(s => s.selected).map(m => m.serviceTypeId),
        issues: elem.issues.filter(r => r.selected).map(m => m.issueTypeId)
      }
    });
    // add user
    if (!this.userId) {
      this.subs.add(
        this.usersService.postCompanyUsers(userObj).subscribe(({ ok, result, message }) => {
          if (ok) {
            message = this.languageService.lang == 'en' ? 'Added successfully !' : 'تم اضافة المستخدم بنجاح !';
            this.snackBar.open(message, 'OK', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-success']
            })
            this.rightDrawer.close();
          }
          else {
            this.snackBar.open(message, 'ERROR', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-danger']
            })
          }
          this.loadingService.itIsLoading = false;
          this.loading = false;
        })
      );
    }
    // edit user
    else {
      userObj.companyUserId = this.userId;
      this.subs.add(
        this.usersService.putCompanyUsers(userObj).subscribe(({ ok, result, message }) => {
          if (ok) {
            message = this.languageService.lang == 'en' ? 'Updated successfully !' : 'تم تعديل المستخدم بنجاح !';
            this.snackBar.open(message, 'OK', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-success']
            })
            this.rightDrawer.close();
          } else {
            this.snackBar.open(message, 'ERROR', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-danger']
            })
          }
          this.loadingService.itIsLoading = false;
          this.loading = false;
        })
      );
    }
  }

  onChangeCompounds(ob: MatCheckboxChange) {
    this.compounds.forEach(element => {
      if (!element.selected) {
        element.services.forEach(serv => serv.selected = false);
        element.issues.forEach(issue => issue.selected = false);
      }
    });
  }

}
