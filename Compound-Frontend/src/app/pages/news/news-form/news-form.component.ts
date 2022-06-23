import { Subscription } from 'rxjs';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { News, NewsService } from 'src/app/services/api/admin/news.service';

import { LanguageService } from 'src/app/services/language.serives';
import { LoadingService } from 'src/app/services/loading.serives';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { CurrentUserService } from './../../../services/current-user.serives';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';

@Component({
  selector: 'app-news-form',
  templateUrl: './news-form.component.html',
  styleUrls: ['./news-form.component.scss']
})
export class NewsFormComponent implements OnInit, OnDestroy {
  compoundId!: string;
  newsId?: string | null;
  form: FormGroup = Object.create(null);
  loading: boolean = false;
  today = new Date().toISOString();

  subs = new Subscription();

  public get SystemPageActions(): typeof SystemPageActions {
    return SystemPageActions;
  }


  constructor(private fb: FormBuilder,
    public rightDrawer: RightDrawer,
    private activatedRoute: ActivatedRoute,
    private newsService: NewsService,
    public currentUserService: CurrentUserService,
    private loadingService: LoadingService,
    private snackBar: MatSnackBar,
    public languageService: LanguageService) {
    this.activatedRoute.paramMap.subscribe((params) => {
      var compoundId = params.get('compoundId');
      if (compoundId != null) {
        this.compoundId = compoundId;
      }
      //var newsId = params.get('newsId');
      // if (newsId != null) {
      //   this.newsId = newsId;
      // }
      this.newsId = this.rightDrawer.parentData;
    });
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      compoundNewsId: [null],
      compoundId: [this.compoundId, Validators.compose([Validators.required])],
      englishTitle: [null, Validators.compose([Validators.required])],
      arabicTitle: [null, Validators.compose([Validators.required])],
      englishSummary: [null, Validators.compose([Validators.required])],
      arabicSummary: [null, Validators.compose([Validators.required])],
      englishDetails: [null, Validators.compose([Validators.required])],
      arabicDetails: [null, Validators.compose([Validators.required])],
      publishDate: [new Date().toISOString(), Validators.compose([Validators.required])],
      publishTime: [new Date(), Validators.compose([Validators.required])],
      foregroundTillDate: [null],
      foregroundTillTime: [null],
      images: [null, Validators.compose([Validators.required])],
    });

    // edit news
    if (this.newsId) {
      this.subs.add(
        this.newsService.getNews(this.newsId).subscribe(({ ok, result, message }) => {
          if (ok) {
            this.form.patchValue({
              compoundNewsId: this.newsId,
              englishTitle: result.englishTitle,
              arabicTitle: result.arabicTitle,
              englishSummary: result.englishSummary,
              arabicSummary: result.arabicSummary,
              englishDetails: result.englishDetails,
              arabicDetails: result.arabicDetails,
              publishDate: result.publishDate,
              publishTime: result.publishDate,
              foregroundTillDate: result.foregroundTillDate,
              foregroundTillTime: result.foregroundTillDate,
              images: result.images
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

  ngOnDestroy() {
    this.subs.unsubscribe();
  }

  onSelectPublishDate(event: any) {
    this.form.patchValue({ foregroundTillDate: null });
  }

  onSelectForegroundTillDate(event: any) {
    if (event.value) {
      this.form.patchValue({ foregroundTillTime: new Date() });
      this.form.get('foregroundTillTime')?.setValidators(Validators.required);
      this.form.get('foregroundTillTime')?.updateValueAndValidity();
    } else {
      this.form.get('foregroundTillTime')?.clearValidators();
      this.form.get('foregroundTillTime')?.updateValueAndValidity();
      this.form.patchValue({ foregroundTillTime: null });
    }
  }

  onSubmit() {
    this.loading = true;
    this.loadingService.itIsLoading = true;
    // merge date with time
    const publishDateTime = this.mergDateTime(new Date(this.form.value.publishDate), this.form.value.publishTime);
    this.form.patchValue({ publishDate: publishDateTime });
    if (this.form.value.foregroundTillDate)
      this.form.patchValue({ foregroundTillDate: this.mergDateTime(this.form.value.foregroundTillDate, this.form.value.foregroundTillTime) });
    let newsObj = new News();
    Object.assign(newsObj, this.form.value);
    newsObj.isActive = true;

    // add news
    if (!this.newsId) {
      this.subs.add(
        this.newsService.postNews(newsObj).subscribe(({ ok, result, message }) => {
          if (ok) {
            message = this.languageService.lang == 'en' ? 'Added successfully !' : 'تم نشر الخبر بنجاح !';
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
    // edit news
    else {
      this.subs.add(
        this.newsService.putNews(newsObj).subscribe(({ ok, result, message }) => {
          if (ok) {
            message = this.languageService.lang == 'en' ? 'Updated successfully !' : 'تم تعديل الخبر بنجاح !';
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

  mergDateTime(date: Date, time: Date) {
    // =====> to merge date with time
    let dateTime = new Date(
      date.getFullYear(),
      date.getMonth(),
      date.getDate(),
      time.getHours(),
      time.getMinutes()
    );
    // =====> to clear time from timezone:
    dateTime = new Date(dateTime);
    const timeZoneDifference = (dateTime.getTimezoneOffset() / 60) * -1;
    dateTime.setTime(dateTime.getTime() + timeZoneDifference * 60 * 60 * 1000);
    return dateTime.toISOString();
  }
}
