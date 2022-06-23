import { trigger, state, style, transition, animate } from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
import { CompoundServiceType, CompoundServicesService, ServiceSubType } from 'src/app/services/api/admin/compound-services.service';
import { ICompound } from 'src/app/services/api/admin/compounds.service';
import { ServiceSubTypesService } from 'src/app/services/api/admin/service-sub-types.service';
import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';

@Component({
  selector: 'app-form-service',
  templateUrl: './form-service.component.html',
  styleUrls: ['./form-service.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})

export class FormServiceComponent implements OnInit {
  subs = new Subscription();
  selectedCompound: ICompound | undefined
  servicesList: CompoundServiceType[] = [];
  loading: boolean = false;

  columnsToDisplay = ['serviceName'];
  expandedElement?: CompoundServiceType;


  constructor(
    public rightDrawer: RightDrawer,
    public dialog: MatDialog,
    private compoundServicesService: CompoundServicesService,
    private subTypesService: ServiceSubTypesService,
    public languageService: LanguageService,
    private translateService: TranslateService,
    private loadingService: LoadingService,
    private snackBar: MatSnackBar) {

  }

  ngOnInit(): void {
    this.selectedCompound = this.rightDrawer.parentData;
    this.populateServices();
  }

  populateServices() {
    if (!this.selectedCompound)
      return;
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.compoundServicesService.getServices(this.selectedCompound?.compoundId).subscribe(({ ok, result, message }) => {
        if (ok) {
          this.servicesList = result;
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

  onChangeServiceType(serviceType: CompoundServiceType) {
    if (serviceType.selected) {
      const message = this.languageService.lang == 'en' ? 'Please add at least one Sub-Service to activate the Service Type.' : 'الرجاء إضافة خدمة فرعية واحدة على الأقل لتفعيل نوع الخدمة.';
      this.snackBar.open(message, 'OK', {
        duration: 3000,
        horizontalPosition: 'end',
        verticalPosition: 'bottom',
        panelClass: ['bg-warning']
      })
    }
    else {
      this.loading = true;
      this.loadingService.itIsLoading = true;
      this.subs.add(
        this.compoundServicesService.updateServices(this.selectedCompound?.compoundId!, [serviceType])
          .subscribe(({ ok, result, message }) => {
            if (ok) {
              this.snackBar.open(this.translateService.instant('services.add.successMsg'),
                this.translateService.instant('services.add.success'), {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-success']
              });
            } else {
              this.snackBar.open(message, 'ERROR', {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-danger']
              })
            }
            this.loadingService.itIsLoading = false;
            this.loading = false;
          }))
    }
  }

  onAddServiceSubType(englishName: string, arabicName: string, cost: string, serviceType: CompoundServiceType) {
    this.loadingService.itIsLoading = true;
    this.loading = true;
    let subTypeToAdd: ServiceSubType = {
      compoundId: this.selectedCompound?.compoundId,
      serviceTypeId: serviceType.serviceTypeId,
      englishName: englishName,
      arabicName: arabicName,
      cost: +cost
    }
    this.subs.add(
      this.subTypesService.postServiceSubType(subTypeToAdd)
        .subscribe(({ ok, result, message }) => {
          if (ok) {
            this.snackBar.open(this.translateService.instant('services.add.successMsg'),
              this.translateService.instant('services.add.success'), {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-success']
            });
            serviceType.serviceSubTypes?.push({
              ...subTypeToAdd,
              serviceSubTypeId: result.serviceSubTypeId,
              serviceTypeId: result.serviceSubTypeId,
              compoundServiceId: result.compoundServiceId
            });
          } else {
            this.snackBar.open(message, 'ERROR', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-danger']
            })
          }
          this.loadingService.itIsLoading = false;
          this.loading = false;
        }))
  }

  onEditServiceSubType(id: string, englishName: string, arabicName: string, cost: string, serviceType: CompoundServiceType) {
    this.loadingService.itIsLoading = true;
    this.loading = true;
    const subTypeToEdit: ServiceSubType = {
      serviceSubTypeId: id,
      compoundId: this.selectedCompound?.compoundId,
      serviceTypeId: serviceType.serviceTypeId,
      englishName: englishName,
      arabicName: arabicName,
      cost: +cost
    }
    this.subs.add(
      this.subTypesService.putServiceSubType(subTypeToEdit)
        .subscribe(({ ok, result, message }) => {
          if (ok) {
            this.snackBar.open(this.translateService.instant('services.add.successMsg'),
              this.translateService.instant('services.add.success'), {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-success']
            });
            serviceType.serviceSubTypes = serviceType.serviceSubTypes!.map(s => {
              if (s.serviceSubTypeId == id) {
                let updatedSubType: ServiceSubType = {
                  ...s,
                  englishName: englishName,
                  arabicName: arabicName,
                  cost: +cost,
                  isEditing: false
                };
                return updatedSubType;
              }
              else {
                return s;
              }
            });
          } else {
            this.snackBar.open(message, 'ERROR', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-danger']
            })
          }
          this.loadingService.itIsLoading = false;
          this.loading = false;
        }))
  }

  onDeleteServiceSubType(event: any, subType: ServiceSubType, serviceType: CompoundServiceType) {
    let targetAttr = event.target.getBoundingClientRect();

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    if (this.languageService.lang == 'en') {
      dialogConfig.data = {
        massage: "Are you sure to delete?"
      };
      dialogConfig.position = {
        top: targetAttr.y + targetAttr.height - 130 + "px",
        left: targetAttr.x - targetAttr.width - 80 + "px"
      };
    } else {
      dialogConfig.data = {
        massage: "هل أنت متأكد من الحذف؟"
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
        this.loading = true;
        this.subs.add(
          this.subTypesService.deleteServiceSubType(subType.serviceSubTypeId!).subscribe(({ ok, result, message }) => {
            if (ok) {
              serviceType.serviceSubTypes?.splice(serviceType.serviceSubTypes.indexOf(subType), 1);
              message = this.languageService.lang == 'en' ? 'Deleted Successfully !' : 'تم الحذف بنجاح !';
              this.snackBar.open(message, 'OK', {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-success']
              })
            } else {
              if (message == 'Cannot delete because it is already linked to Service Requests!' && this.languageService.lang == 'ar') {
                message = 'لا يمكن حذف هذه الخدمة الفرعية لأنها مرتبطة ببعض طلبات الخدمات بالفعل!'
              }
              this.snackBar.open(message, 'ERROR', {
                duration: 5000,
                horizontalPosition: 'start',
                panelClass: ['bg-danger']
              })
            }
            this.loadingService.itIsLoading = false;
            this.loading = false;
          })
        );
      }
    });
  }
}
