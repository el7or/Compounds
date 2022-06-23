import { RightDrawer } from './../../../services/right-drawer.serives';
import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { CompoundServicesService, ServiceRequest } from 'src/app/services/api/admin/compound-services.service';
import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';
import { Location } from '@angular/common';

@Component({
  selector: 'app-service-request',
  templateUrl: './service-request.component.html',
  styleUrls: ['./service-request.component.scss']
})
export class ServiceRequestComponent implements OnInit {

  loading: boolean = false;
  requestId?: string;
  compoundId?: string;
  subs = new Subscription();
  serviceRequest!: ServiceRequest;
  audio = new Audio();
  constructor(private activatedRoute: ActivatedRoute,
    private compoundServicesService: CompoundServicesService,
    private loadingService: LoadingService,
    private snackBar: MatSnackBar,
    public languageService: LanguageService,
    public rightDrawer: RightDrawer,
    private dialog: MatDialog,
    public location: Location) {
    this.activatedRoute.paramMap.subscribe((params) => {
      var compoundId = params.get('compoundId');
      var requestId = params.get('requestId');
      if (compoundId != null) {
        this.compoundId = compoundId;
      }
      this.requestId = this.rightDrawer.parentData;
    });
  }

  ngOnInit(): void {
    this.getRequest();
  }

  getRequest(): void {
    if (!this.requestId) return;
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.compoundServicesService.getRequest(this.requestId).subscribe(({ ok, result, message }) => {
        if (ok) {
          this.serviceRequest = result;
          if (result.record) {
            this.audio.src = result.record;
          }
        } else {
          this.snackBar.open(message, 'ERROR', {
            duration: 3000,
            horizontalPosition: 'start',
            panelClass: ['bg-danger']
          })
        }
        this.loadingService.itIsLoading = false;
      })
    );
  }

  openCancelDialog(event: any, requestId: string) {
    let targetAttr = event.target.getBoundingClientRect();

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    if (this.languageService.lang == 'en') {
      dialogConfig.data = {
        massage: "Are you sure to cancel this request?"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width - 80 + "px"
      };
    } else {
      dialogConfig.data = {
        massage: "هل أنت متأكد من إلغاء هذا الطلب"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width + 80 + "px"
      };
    }

    dialogConfig.backdropClass = 'dialog-bg-trans';
    const dialogRef = this.dialog.open(DialogLayoutComponent, dialogConfig
    );

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadingService.itIsLoading = true;
        this.subs.add(
          this.compoundServicesService.updateRequestStatus(requestId, { status: 2 })
            .subscribe(({ ok, result, message }) => {
              if (ok) {
                message = this.languageService.lang == 'en' ? 'Cancelled Successfully !' : 'تم الإلغاء بنجاح !';
                this.snackBar.open(message, 'OK', {
                  duration: 3000,
                  horizontalPosition: 'start',
                  panelClass: ['bg-success']
                });
                this.getRequest();
                this.rightDrawer.close();
              } else {
                this.snackBar.open(message, 'ERROR', {
                  duration: 3000,
                  horizontalPosition: 'start',
                  panelClass: ['bg-danger']
                })
              }
              this.loadingService.itIsLoading = false;
            })
        );
      }
    });
  }

  openDoneDialog(event: any, requestId: string) {
    let targetAttr = event.target.getBoundingClientRect();

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    if (this.languageService.lang == 'en') {
      dialogConfig.data = {
        massage: "Are you sure to mark this request as done?"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width - 80 + "px"
      };
    } else {
      dialogConfig.data = {
        massage: "هل أنت متأكد من تحديد هذا الطلب بأنه تم إنجازه ؟"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width + 80 + "px"
      };
    }

    dialogConfig.backdropClass = 'dialog-bg-trans';
    const dialogRef = this.dialog.open(DialogLayoutComponent, dialogConfig
    );

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadingService.itIsLoading = true;
        this.subs.add(
          this.compoundServicesService.updateRequestStatus(requestId, { status: 1 })
            .subscribe(({ ok, result, message }) => {
              if (ok) {
                message = this.languageService.lang == 'en' ? 'Done Successfully !' : 'تم تعديل الطلب بنجاح !';
                this.snackBar.open(message, 'OK', {
                  duration: 3000,
                  horizontalPosition: 'start',
                  panelClass: ['bg-success']
                });
                this.getRequest();
                this.rightDrawer.close();
              } else {
                this.snackBar.open(message, 'ERROR', {
                  duration: 3000,
                  horizontalPosition: 'start',
                  panelClass: ['bg-danger']
                })
              }
              this.loadingService.itIsLoading = false;
            })
        );
      }
    });
  }

  sendComment(): void {
    if (!this.requestId) return;
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.compoundServicesService.updateRequestComment(this.requestId,
        { comment: this.serviceRequest.comment })
        .subscribe(({ ok, result, message }) => {
          if (ok) {
            message = this.languageService.lang == 'en' ? 'Comment Added Successfully !' : 'تم تعديل التعليق بنجاح !';
            this.snackBar.open(message, 'OK', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-success']
            });
            this.getRequest();
            this.rightDrawer.close();
          } else {
            this.snackBar.open(message, 'ERROR', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-danger']
            })
          }
          this.loadingService.itIsLoading = false;
        })
    );
  }

  sendAndDone(): void {
    if (!this.requestId) return;
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.compoundServicesService.updateRequestComment(this.requestId,
        { comment: this.serviceRequest.comment, status: 1 })
        .subscribe(({ ok, result, message }) => {
          if (ok) {
            message = this.languageService.lang == 'en' ? 'Updated Successfully !' : 'تم التعديل بنجاح !';
            this.snackBar.open(message, 'OK', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-success']
            });
            this.getRequest();
            this.rightDrawer.close();
          } else {
            this.snackBar.open(message, 'ERROR', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-danger']
            })
          }
          this.loadingService.itIsLoading = false;
        })
    );
  }

  playRecord(): void {
    this.audio.load();
    this.audio.play();
  }

}
