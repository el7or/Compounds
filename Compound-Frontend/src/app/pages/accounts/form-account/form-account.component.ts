import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import {
  AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors,
  Validators
} from '@angular/forms';
import { GoogleMap } from '@angular/google-maps';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { CompaniesService } from 'src/app/services/api/admin/companies.service';
import { LanguageService } from 'src/app/services/language.serives';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-form-account',
  templateUrl: './form-account.component.html',
  styleUrls: ['./form-account.component.scss']

})
export class FormAccountComponent implements OnInit {
  currentLang: string = '';
  @ViewChild('googleMap', { static: false }) map!: GoogleMap;
  apiLoaded: Observable<boolean>;
  options: google.maps.MapOptions = { zoom: 6 };
  isEditable = false;
  center: google.maps.LatLngLiteral = { lat: 26.8206, lng: 30.8025 };
  markerOptions: google.maps.MarkerOptions = { draggable: false };
  markerLocation: google.maps.LatLngLiteral = this.center;

  visibility1: boolean = false;
  visibility2: boolean = false;
  loading: boolean = false;
  private webSiteReg = '(https?://)?([\\da-z_.-]+)\\.([a-z.]{2,6})[/\\w .-]*/?';
  private passwReg = '((?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{8,})';
  comanyInfoForm: FormGroup = new FormGroup({
    logo: new FormControl(null, [Validators.required]),
    name_Ar: new FormControl(null, [Validators.required, Validators.minLength(3)]),
    name_En: new FormControl(null, [Validators.required, Validators.minLength(3)]),
    website: new FormControl(null, [Validators.pattern(this.webSiteReg)]),
  });
  comanyLocationForm: FormGroup = new FormGroup({
    location: new FormControl(null, [Validators.required]),
    address: new FormControl(null, [Validators.required]),
  });
  contactInfoForm: FormGroup = new FormGroup({
    phone: new FormControl(null),
    chargeName: new FormControl(null, [Validators.required]),
  });
  loginInfoFrom: FormGroup = new FormGroup({
    email: new FormControl(null, [Validators.required, Validators.email]),
    password: new FormControl(null, [Validators.required,
    Validators.pattern(this.passwReg)]),
    confirmPassword: new FormControl(null, [Validators.required]),
    planId: new FormControl(null),
  });

  confirmPasswordValidator(): boolean {
    let isValid: boolean = this.loginInfoFrom.value.password === this.loginInfoFrom.value.confirmPassword;
    if (isValid) {
      this.loginInfoFrom.get('password')?.setErrors(null);
      this.loginInfoFrom.get('confirmPassword')?.setErrors(null);
    } else {
      this.loginInfoFrom.get('password')?.setErrors({ 'incorrect': true });
      this.loginInfoFrom.get('confirmPassword')?.setErrors({ 'incorrect': true });
    }
    // this.loginInfoFrom.controls.password.updateValueAndValidity();
    // this.loginInfoFrom.controls.confirmPassword.updateValueAndValidity();
    return isValid;
  }

  constructor(
    // private _formBuilder: FormBuilder,
    private httpClient: HttpClient,
    private companiesService: CompaniesService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar,
    public languageService: LanguageService) {
    if (this.languageService.lang == 'en') {
      this.currentLang = 'العربية';
    } else {
      this.currentLang = 'English';
    }
    this.apiLoaded = this.httpClient.jsonp(environment.googeMapUrl, 'callback')
      .pipe(
        map(() => {
          setTimeout(() => {
            this.map.mapDrag.subscribe(() => this.markerLocation = this.map.getCenter().toJSON());
            this.map.mapDragend.subscribe(() => {
              this.markerLocation = this.map.getCenter().toJSON();
              this.comanyLocationForm.get('location')?.setValue({
                latitude: this.markerLocation.lat,
                longitude: this.markerLocation.lng
              })
            });
          })
          return true;
        }),
        catchError(() => of(false)),
      );
  }


  changeLang() {
    if (this.languageService.lang == 'en') {
      this.languageService.selectLang('ar');
    } else {
      this.languageService.selectLang('en');
    }
  }
  ngOnInit() {
    this.activatedRoute.params.subscribe(({ planId }) => this.loginInfoFrom.get('planId')?.setValue(planId));
  }
  submit() {
    this.loading = true;
    let fromValue = {
      ...this.comanyInfoForm.value,
      ...this.comanyLocationForm.value,
      ...this.contactInfoForm.value,
      ...this.loginInfoFrom.value
    }
    if (this.confirmPasswordValidator()) {
      this.companiesService.postCompanies(fromValue).subscribe(({ message, ok, result }) => {
        if (ok) {
          this.router.navigate(['/auth/login']);
          this.loading = false;
        } else {
          this.snackBar.open(message, 'OK', {
            duration: 3000,
            horizontalPosition: 'start',
            panelClass: ['bg-danger']
          });
        }
      })
    }
  }
}
