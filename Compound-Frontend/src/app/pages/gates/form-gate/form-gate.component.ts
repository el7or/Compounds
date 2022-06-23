import { GatesService } from './../../../services/api/admin/gates.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { ActivatedRoute } from '@angular/router';
import {  ICompound } from 'src/app/services/api/admin/compounds.service';
import { CompoundsStoreService } from 'src/app/services/compounds-store.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { observable, Observable, Subscription } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { CurrentUserService } from './../../../services/current-user.serives';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';


@Component({
  selector: 'app-form-gate',
  templateUrl: './form-gate.component.html',
  styleUrls: ['./form-gate.component.scss']
})

export class FormGateComponent implements OnInit {
  selectedCompound: ICompound | undefined;
  entryTypes=[{value:1,name:'Entrance'},{value:2,name:'Exit'},{value:3,name:'All'}] ;
  visibility1: boolean = false;
  visibility2: boolean = false;
  loading: boolean = false;
  allGates: any[] = [];
  filteredGates : Observable<any[]> = new Observable<[]>() ;
  gateId?: string | null;
  subs = new Subscription();
  gateForm: FormGroup = new FormGroup({
    //gateId: new FormControl(null ),
    gateName: new FormControl(null, [Validators.required]),
    entryType: new FormControl(null, [Validators.required]),
    compoundIds: new FormControl(null, [Validators.required]),
    userName: new FormControl(null, [Validators.required]),
    password: new FormControl(null, [Validators.required]),
    confirmPassword: new FormControl(null,[Validators.required]),
    gateId: new FormControl(null)
  });

  changeValue(value:any){
    this.gateForm.patchValue(
      {
        entryType:value
      }
    )
  }
  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }
  constructor(
    public rightDrawer: RightDrawer,
    public dialog: MatDialog,
    private gateService:GatesService,
    public currentUserService: CurrentUserService,
    private snackBar: MatSnackBar,
    private compoundsStoreService: CompoundsStoreService,
    private activatedRoute:ActivatedRoute,
    public languageService: LanguageService,
    private gatesService:GatesService
  ) {
    this.activatedRoute.params.subscribe(({ compoundId }) => {
      this.selectedCompound = this.compoundsStoreService.compounds?.
               find(i => i.compoundId == compoundId);
                    this.gateForm.patchValue({
                      compoundIds: [this.selectedCompound?.compoundId],
                    });
              });
    this.gateId = this.rightDrawer.parentData?.gateId;
  if(this.rightDrawer.parentData?.compoundId){
    this.gateForm.patchValue({
      compoundIds: [this.rightDrawer.parentData?.compoundId],
    });
    }
   }

  ngOnInit(): void {
    this.gatesService.getAllGates().subscribe(({ ok, result, message }) => {
      if (ok) {
        this.allGates = result;
      }else{
        this.snackBar.open(message, 'OK', {
          duration: 3000,
          horizontalPosition: 'start',
          panelClass: ['bg-danger']
        })
      }
  });
    this.filteredGates = this.gateForm.controls.gateName.valueChanges
    .pipe(
      startWith(''),
      map(value => this._filter(value))
    );

    if (this.gateId) {
      this.subs.add(
        this.gatesService.getGate(this.gateId).subscribe(({ ok, result, message }) => {
          if (ok) {
            this.gateForm.patchValue({
              gateId: this.gateId,
              gateName: result.gateName,
              userName: result.userName,
              password: result.password,
              entryType: result.entryType
            });
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

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.allGates.filter(option => option.gateName.toLowerCase().includes(filterValue));
  }

  confirmPasswordValidator(): boolean {
    let isValid: boolean = this.gateForm.value.password === this.gateForm.value.confirmPassword;
    if (isValid) {
      this.gateForm.get('password')?.setErrors(null);
      this.gateForm.get('confirmPassword')?.setErrors(null);
    } else {
      this.gateForm.get('password')?.setErrors({ 'incorrect': true });
      this.gateForm.get('confirmPassword')?.setErrors({ 'incorrect': true });
    }
    return isValid;
  }

  submit() {
    let Formm = JSON.stringify(this.gateForm.value);
    const gate =this.gateForm.value['gateName'];
    const selectedGate = this.allGates.find(x=>x.gateName == gate);
    if(!this.confirmPasswordValidator())
    return;
    if(selectedGate)
      this.gateForm.patchValue({gateId: selectedGate.gateId});
    this.loading = true;
    if (!this.gateId)
    {
        this.gateService.postGates(Formm).subscribe(({ ok, message, result }) => {
          this.loading = false;
          if (ok) {
            this.rightDrawer.close(result);
         }else{
          this.snackBar.open(message, 'OK', {
            duration: 3000,
            horizontalPosition: 'start',
            panelClass: ['bg-danger']
          })
       }
   });
  }
  else{
    this.gateService.putGates(Formm).subscribe(({ ok, message, result }) => {
      this.loading = false;
      if (ok) {
        this.rightDrawer.close(result);
     }else{
      this.snackBar.open(message, 'OK', {
        duration: 3000,
        horizontalPosition: 'start',
        panelClass: ['bg-danger']
      })
   }
});
  }
  }
}
