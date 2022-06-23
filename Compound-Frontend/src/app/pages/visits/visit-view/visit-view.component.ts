import { LanguageService } from './../../../services/language.serives';
import { Subscription } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { VisitDay, VisitDetails, VisitStatus, VisitType, VisitsService, VisitRequestApprove } from 'src/app/services/api/admin/visits.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OwnerType } from 'src/app/services/api/admin/sub-owners.service';
import { CurrentUserService } from './../../../services/current-user.serives';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';


@Component({
  selector: 'app-visit-view',
  templateUrl: './visit-view.component.html',
  styleUrls: ['./visit-view.component.scss']
})
export class VisitViewComponent implements OnInit {
  visitDetails!: VisitDetails;
  loading = false;
  form: FormGroup = Object.create(null);

  subs = new Subscription();
  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }
  constructor(public rightDrawer: RightDrawer,
    private fb: FormBuilder,
    public currentUserService: CurrentUserService,
    private languageService: LanguageService,
    private snackBar: MatSnackBar,
    private visitsService: VisitsService) { }

  ngOnInit(): void {
    this.visitDetails = this.rightDrawer.parentData
    if (this.visitDetails) {
      this.visitDetails.status = VisitStatus[this.visitDetails.status];
      this.visitDetails.type = VisitType[this.visitDetails.type];
      this.visitDetails.userType = OwnerType[this.visitDetails.userType];
      this.visitDetails.days = this.visitDetails.days.map(day => {
        day = VisitDay[day];
        return day;
      });
      if (this.visitDetails.type == 'Labor') {
        this.form = this.fb.group({
          attachments: [this.visitDetails.attachments, Validators.compose([Validators.required])],
        });
      }
    }
  }

  onConfirm() {
    this.loading = true;
    const visitConfirm: VisitRequestApprove = {
      id: this.visitDetails.visitRequestId,
      isApproved: true,
      attachments: this.visitDetails.type == 'Labor' ? this.form.value.attachments : null
    }
    this.subs.add(
      this.visitsService.confirmVisit(visitConfirm).subscribe(({ ok, result, message }) => {
        if (ok) {
          message = this.languageService.lang == 'en' ? 'Confirmed successfully !' : 'تم تأكيد الزيارة بنجاح !';
          this.snackBar.open(message, 'OK', {
            duration: 3000,
            horizontalPosition: 'start',
            panelClass: ['bg-success']
          })
          this.rightDrawer.close(result);
        } else {
          this.snackBar.open(message, 'ERROR', {
            duration: 3000,
            horizontalPosition: 'start',
            panelClass: ['bg-danger']
          })
        }
        this.loading = false;
      })
    );
  }
}
