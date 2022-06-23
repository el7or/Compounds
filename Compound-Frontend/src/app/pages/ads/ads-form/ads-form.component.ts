import { IGroup } from './../../../services/api/admin/compound-groups.service';
import { Ad } from './../../../services/api/admin/ads.service';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { AdsService } from 'src/app/services/api/admin/ads.service';
import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { MatRadioChange } from '@angular/material/radio';
import { CurrentUserService } from './../../../services/current-user.serives';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';

@Component({
  selector: 'app-ads-form',
  templateUrl: './ads-form.component.html',
  styleUrls: ['./ads-form.component.scss']
})
export class AdsFormComponent implements OnInit, OnDestroy {
  compoundId!: string;
  adId?: string;
  form: FormGroup = Object.create(null);
  loading: boolean = false;
  today = new Date().toISOString();
  urlReg = '(https?://)?([\\da-z.-]+)\\.([a-z.]{2,6})[/\\w .-]*/?';

  subs = new Subscription();
  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }


  constructor(private fb: FormBuilder,
    public rightDrawer: RightDrawer,
    private activatedRoute: ActivatedRoute,
    private adService: AdsService,
    public currentUserService: CurrentUserService,
    private loadingService: LoadingService,
    private snackBar: MatSnackBar,
    public languageService: LanguageService) {
    this.activatedRoute.paramMap.subscribe((params) => {
      var compoundId = params.get('compoundId');
      if (compoundId != null) {
        this.compoundId = compoundId;
      }
      this.adId = this.rightDrawer.parentData;
    });
  }

  ngOnInit(): void {
    this.initForm();

    //if edit ad
    if (this.adId) this.patchForm();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  initForm() {
    this.form = this.fb.group({
      compoundAdId: [null],
      compoundId: [this.compoundId, Validators.compose([Validators.required])],
      startDate: [null, Validators.compose([Validators.required])],
      endDate: [null, Validators.compose([Validators.required])],
      isUrl: [null, Validators.compose([Validators.required])],
      adUrl: [null],
      englishTitle: [null],
      arabicTitle: [null],
      englishDescription: [null],
      arabicDescription: [null],
      images: [null, Validators.compose([Validators.required])],
    });
  }

  patchForm() {
    this.subs.add(
      this.adService.getAdById(this.adId!).subscribe(({ ok, result, message }) => {
        if (ok) {
          this.form.patchValue({
            compoundAdId: this.adId,
            compoundId: this.compoundId,
            startDate: result.startDate,
            endDate: result.endDate,
            isUrl: result.isUrl,
            adUrl: result.adUrl,
            englishTitle: result.englishTitle,
            arabicTitle: result.arabicTitle,
            englishDescription: result.englishDescription,
            arabicDescription: result.arabicDescription,
            images: result.images
          });
        } else {
          this.snackBar.open(message, 'ERROR', {
            duration: 3000,
            horizontalPosition: 'start',
            panelClass: ['bg-danger']
          });
        }
      })
    );
  }

  onSelectStartDate(event: any) {
    this.form.patchValue({ endDate: null })
  }

  onChooseType(event: MatRadioChange) {
    if (event.value) {
      this.form.get('adUrl')?.setValidators([Validators.required, Validators.pattern(this.urlReg)]);
      this.form.get('adUrl')?.updateValueAndValidity();

      this.form.get('englishTitle')?.clearValidators();
      this.form.get('englishTitle')?.updateValueAndValidity();

      this.form.get('arabicTitle')?.clearValidators();
      this.form.get('arabicTitle')?.updateValueAndValidity();

      this.form.get('englishDescription')?.clearValidators();
      this.form.get('englishDescription')?.updateValueAndValidity();

      this.form.get('arabicDescription')?.clearValidators();
      this.form.get('arabicDescription')?.updateValueAndValidity();
    } else {
      this.form.get('adUrl')?.clearValidators();
      this.form.get('adUrl')?.updateValueAndValidity();

      this.form.get('englishTitle')?.setValidators(Validators.required);
      this.form.get('englishTitle')?.updateValueAndValidity();

      this.form.get('arabicTitle')?.setValidators(Validators.required);
      this.form.get('arabicTitle')?.updateValueAndValidity();

      this.form.get('englishDescription')?.setValidators(Validators.required);
      this.form.get('englishDescription')?.updateValueAndValidity();

      this.form.get('arabicDescription')?.setValidators(Validators.required);
      this.form.get('arabicDescription')?.updateValueAndValidity();
    }
  }

  onSubmit() {
    this.loading = true;
    this.loadingService.itIsLoading = true;
    let adObj = new Ad();
    Object.assign(adObj, this.form.value);
    adObj.startDate = new Date(this.form.value.startDate).toDateString();
    adObj.endDate = new Date(this.form.value.endDate).toDateString();
    adObj.isActive = true;

    // add ad
    if (!this.adId) {
      this.subs.add(
        this.adService.postAd(adObj).subscribe(({ ok, result, message }) => {
          if (ok) {
            message = this.languageService.lang == 'en' ? 'Sent successfully !' : 'تم حفظ الإعلان بنجاح !';
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
    // edit ad
    else {
      this.subs.add(
        this.adService.putAd(adObj).subscribe(({ ok, result, message }) => {
          if (ok) {
            message = this.languageService.lang == 'en' ? 'Updated successfully !' : 'تم تعديل الإعلان بنجاح !';
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
}
