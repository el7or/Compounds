import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subscription } from 'rxjs';
import { CompoundIssuesService } from 'src/app/services/api/admin/compound-issues.service';
import { ICompound } from 'src/app/services/api/admin/compounds.service';
import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';

@Component({
  selector: 'app-form-issue',
  templateUrl: './form-issue.component.html',
  styleUrls: ['./form-issue.component.scss']
})
export class FormIssueComponent implements OnInit {
  subs = new Subscription();
  selectedCompound: ICompound | undefined
  issuesList: any[] = [];

  loading: boolean = false;


  constructor(
    public rightDrawer: RightDrawer,
    public dialog: MatDialog,
    private compoundIssuesService: CompoundIssuesService,
    public languageService: LanguageService,
    private loadingService: LoadingService,
    private snackBar: MatSnackBar) {

  }

  ngOnInit(): void {
    this.selectedCompound = this.rightDrawer.parentData;
    this.populateIssues();
  }

  populateIssues() {
    if (!this.selectedCompound)
      return;
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.compoundIssuesService.getIssues(this.selectedCompound?.compoundId).subscribe(({ ok, result, message }) => {
        if (ok) {
          this.issuesList = result;
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

  openDialog() {
    this.dialog.open(DialogLayoutComponent, {
      data: 'Unit added successfully'
    });
  }

  saveIssues() {
    if (!this.selectedCompound)
      return;
    const selectedIssues = this.issuesList.filter(z => z.selected);
    if (selectedIssues.length == 0) {
      var message = this.languageService.lang == 'en' ? 'You should select minimum one issue' : 'يجب على الأقل اختيار مشكلة واحدة';
      this.snackBar.open(message, 'WARNING', {
        duration: 3000,
        horizontalPosition: 'start',
        panelClass: ['bg-danger']
      });
      return;
    }
    this.loadingService.itIsLoading = true;
    this.subs.add(
      this.compoundIssuesService.updateIssues(this.selectedCompound?.compoundId, this.issuesList)
        .subscribe(({ ok, result, message }) => {
          if (ok) {
            message = this.languageService.lang == 'en' ? 'Compound Issues Updated Successfully' : 'تم حفظ مشكلات الكمباوند بنجاح';
            this.snackBar.open(message, 'OK', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-success']
            });
            this.rightDrawer.close(result);
          } else {
            this.snackBar.open(message, 'ERROR', {
              duration: 3000,
              horizontalPosition: 'start',
              panelClass: ['bg-danger']
            })
          }
          this.loadingService.itIsLoading = false;
        }))
  }
}
