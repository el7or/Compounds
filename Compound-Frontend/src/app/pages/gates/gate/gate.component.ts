import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { ICompound, CompoundsService } from 'src/app/services/api/admin/compounds.service';
import { GatesService, IGate } from 'src/app/services/api/admin/gates.service';
import { CompoundsStoreService } from 'src/app/services/compounds-store.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';
import { FormGateComponent } from '../form-gate/form-gate.component';
import { CurrentUserService } from './../../../services/current-user.serives';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';

@Component({
  selector: 'app-gate',
  templateUrl: './gate.component.html',
  styleUrls: ['./gate.component.scss']
})
export class GateComponent implements OnInit {
  compounds: ICompound[] = [];
  gates: IGate[] = [];
  selectedCompound: ICompound | undefined

  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }

  constructor(
    public rightDrawer: RightDrawer,
    private snackBar: MatSnackBar,
    public languageService: LanguageService,
    private compoundsStoreService: CompoundsStoreService,
    public dialog:MatDialog,
    private activatedRoute:ActivatedRoute,
    private gatesService:GatesService,
    private compoundsService: CompoundsService,
    public currentUserService: CurrentUserService
  ) { }


  ngOnInit() {
    this.activatedRoute.params.subscribe(({ compoundId }) => {
      this.selectedCompound = this.compoundsStoreService.
      compounds?.find(i => i.compoundId == compoundId);
      this.getCompoundGates(compoundId)
    })}

  openFormGate(data: any | null = null) {
    this.rightDrawer.open(FormGateComponent, data).subscribe((arg: any) => {
      if (arg != null && this.selectedCompound) {
        this.getCompoundGates(this.selectedCompound.compoundId);
      }
    })
  }


  deleteGate(gateId:String) {
    this.dialog.open(DialogLayoutComponent, { data: { massage: 'gate.list.deleteConfirm' } })
      .afterClosed().subscribe((confirm) => {
        if (confirm && this.selectedCompound) {
          this.gatesService.DeleteGates(gateId.toString(),this.selectedCompound.compoundId).subscribe(({ ok, result, message }) => {
            if (ok) {
              var notify = this.languageService.lang == 'en' ? 'Gate deleted successfully !' : 'تم حذف البوابة بنجاح !';
              this.snackBar.open(notify, 'OK', {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-primary']
              });
              if(this.selectedCompound)
                this.getCompoundGates(this.selectedCompound.compoundId);
            } else {
              this.snackBar.open(message, 'OK', {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-danger']
              });
            }
          })
        }
      });
  }

  getCompoundGates(compoundId:string):void{
    this.gatesService.getGates(compoundId).subscribe(({ ok, result, message }) => {
      if (ok) {
        this.gates = result;
      }else{
        this.snackBar.open(message, 'OK', {
          duration: 3000,
          horizontalPosition: 'start',
          panelClass: ['bg-danger']
        })
      }
  })
  }

}
