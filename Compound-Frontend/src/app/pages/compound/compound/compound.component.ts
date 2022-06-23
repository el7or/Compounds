import { DialogLayoutComponent } from './../../../shared/dialog-layout/dialog-layout.component';
import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CompoundsService, ICompound } from 'src/app/services/api/admin/compounds.service';
import { CompoundsStoreService } from 'src/app/services/compounds-store.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { FormGateComponent } from '../../gates/form-gate/form-gate.component';
import { FormServiceComponent } from '../../services/form-service/form-service.component';
import { FormCompoundComponent } from '../form-compound/form-compound.component';
import { MatDialog } from '@angular/material/dialog';
import { FormIssueComponent } from '../../issues/form-issue/form-issue.component';

import { SystemPageActions } from 'src/app/services/system-page-actions.enum';

import { CurrentUserService } from 'src/app/services/current-user.serives';



@Component({
  selector: 'app-compound',
  templateUrl: './compound.component.html',
  styleUrls: ['./compound.component.scss']
})
export class CompoundComponent implements OnInit {
  compounds: ICompound[] = [];


  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }


  constructor(
    public rightDrawer: RightDrawer,
    private compoundsService: CompoundsService,
    private snackBar: MatSnackBar,
    public languageService: LanguageService,
    private compoundsStoreService: CompoundsStoreService,
    public currentUserService: CurrentUserService,
    private loadingService: LoadingService,
    public dialog: MatDialog,

  ) { }

  ngOnInit(): void {
    this.getUserCompounds();
  }

  getUserCompounds() {
    this.loadingService.itIsLoading = true;
    this.compoundsService.getCompounds().subscribe(({ ok, result, message }) => {
      this.loadingService.itIsLoading = false;
      if (ok) {
        const filteredCompounds: ICompound[] = result;
        this.compoundsStoreService.compounds = this.compounds = filteredCompounds
          .filter(compound => this.currentUserService.currentUser?.accessDetails?.compounds?.some(comp => comp.compoundId == compound.compoundId));
      } else {
        this.snackBar.open(message, 'OK', {
          duration: 3000,
          horizontalPosition: 'start',
          panelClass: ['bg-danger']
        })
      }
    });
  }

  deleteGroup(compound: ICompound) {
    this.dialog.open(DialogLayoutComponent, { data: { massage: 'tree.deleteCompound' } })
      .afterClosed().subscribe((confirm) => {
        if (confirm) {
          this.compoundsService.DeleteCompounds(compound.compoundId).subscribe(({ ok, result, message }) => {
            if (ok) {
              this.snackBar.open(result, 'OK', {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-primary']
              })
              this.getUserCompounds();
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

  errorHandler(event: any) {
    console.debug(event);
    event.target.src = "/assets/images/House searching-bro.png ";
  }

  openFormCompound(compound: ICompound | null = null) {
    this.rightDrawer.open(FormCompoundComponent, compound).subscribe((arg: any) => {
      if (arg != null) {
        this.getUserCompounds();
      }
    });
  }

  openFormService(compound: ICompound | null = null) {
    this.rightDrawer.open(FormServiceComponent, compound).subscribe((arg: any) => {
      if (arg != null) {
        this.getUserCompounds();
      }
    });
  }

  openFormIssue(compound: ICompound | null = null) {
    this.rightDrawer.open(FormIssueComponent, compound).subscribe((arg: any) => {
      if (arg != null) {
        this.getUserCompounds();
      }
    });
  }

  openFormGate(compound: ICompound | null = null) {
    this.rightDrawer.open(FormGateComponent, compound).subscribe((arg: any) => {
      if (arg != null) {
        this.getUserCompounds();
      }
    });
  }
}
