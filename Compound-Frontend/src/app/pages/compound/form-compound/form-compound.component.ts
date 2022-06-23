import { LanguageService } from 'src/app/services/language.serives';
import { ITimeZone, TimeZoneService } from './../../../services/time-zone.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { HttpClient } from '@angular/common/http';
import { GoogleMap } from '@angular/google-maps';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { CompoundsService } from 'src/app/services/api/admin/compounds.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CurrentUserService } from 'src/app/services/current-user.serives';
import { MatSelectChange } from '@angular/material/select';
import { Iimage } from 'src/app/services/webApi.service';

@Component({
  selector: 'app-form-compound',
  templateUrl: './form-compound.component.html',
  styleUrls: ['./form-compound.component.scss']
})
export class FormCompoundComponent implements OnInit {
  @ViewChild('googleMap', { static: false }) map!: GoogleMap;
  apiLoaded: Observable<boolean>;
  options: google.maps.MapOptions = { zoom: 6 };
  center: google.maps.LatLngLiteral = { lat: 26.8206, lng: 30.8025 };
  markerOptions: google.maps.MarkerOptions = { draggable: false };
  markerLocation: google.maps.LatLngLiteral = this.center;
  loading: boolean = false;
  timeZones: ITimeZone[] = [];

  compoundNameImageForm: FormGroup = new FormGroup({
    compoundId: new FormControl(null),
    companyId: new FormControl(this.currentUserService.currentUser.companyId),
    image: new FormControl(null, [Validators.required]),
    nameEn: new FormControl(null, [Validators.required, Validators.minLength(3)]),
    nameAr: new FormControl(null, [Validators.required, Validators.minLength(3)]),
  });
  compoundLocationForm: FormGroup = new FormGroup({
    location: new FormControl(null),
    address: new FormControl(null, [Validators.required]),
    timeZoneText: new FormControl(null, [Validators.required]),
    timeZoneOffset: new FormControl(null, [Validators.required]),
    timeZoneValue: new FormControl(null, [Validators.required])
  });
  compoundContactForm: FormGroup = new FormGroup({
    phone: new FormControl(null, [Validators.required]),
    emergencyPhone: new FormControl(null),
    mobile: new FormControl(null, [Validators.required]),
    email: new FormControl(null, [Validators.email])
  });

  constructor(
    public rightDrawer: RightDrawer,
    public httpClient: HttpClient,
    public compoundService: CompoundsService,
    private timeZoneService: TimeZoneService,
    private snackBar: MatSnackBar,
    private languageService: LanguageService,
    private currentUserService: CurrentUserService
  ) {
    this.apiLoaded = this.httpClient.jsonp(environment.googeMapUrl, 'callback')
      .pipe(
        map(() => {
          setTimeout(() => {
            this.map.mapDrag.subscribe(() => this.markerLocation = this.map.getCenter().toJSON());
            this.map.mapDragend.subscribe(() => {
              this.markerLocation = this.map.getCenter().toJSON();
              this.compoundLocationForm.get('location')?.setValue({
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

  ngOnInit(): void {
    this.timeZoneService.getAllTimeZones().subscribe(allTimeZones => this.timeZones = allTimeZones);

    if (this.rightDrawer.parentData) {
      this.compoundNameImageForm.patchValue({
        compoundId: this.rightDrawer.parentData.compoundId,
        companyId: this.rightDrawer.parentData.companyId,
        image: this.rightDrawer.parentData.image,
        nameEn: this.rightDrawer.parentData.nameEn,
        nameAr: this.rightDrawer.parentData.nameAr,
      });
      this.compoundLocationForm.patchValue({
        location: this.rightDrawer.parentData.location,
        address: this.rightDrawer.parentData.address,
        timeZoneText: this.rightDrawer.parentData.timeZoneText,
        timeZoneOffset: this.rightDrawer.parentData.timeZoneOffset,
        timeZoneValue: this.rightDrawer.parentData.timeZoneValue
      });
      this.compoundContactForm.patchValue({
        phone: this.rightDrawer.parentData.phone,
        emergencyPhone: this.rightDrawer.parentData.emergencyPhone,
        mobile: this.rightDrawer.parentData.mobile,
        email: this.rightDrawer.parentData.email,
      });
    }
  }

  onSelectTimeZone(event: MatSelectChange) {
    const text = event.source.triggerValue;
    const timeZoneObj = this.timeZones.find(x => x.text == text);
    this.compoundLocationForm.patchValue({ timeZoneText: text });
    this.compoundLocationForm.patchValue({ timeZoneOffset: timeZoneObj?.offset });
    this.compoundLocationForm.patchValue({ timeZoneValue: timeZoneObj?.value });
  }

  submit() {
    let fromValue = {
      ...this.compoundNameImageForm.value,
      ...this.compoundLocationForm.value,
      ...this.compoundContactForm.value
    }
    this.loading = true;
    let service: any;
    if (fromValue.compoundId) {
      if (typeof fromValue.image == 'string') {
        const newImage: Iimage = { path: fromValue.image };
        fromValue.image = newImage;
      }
      service = this.compoundService.putCompounds(fromValue);
    } else {
      service = this.compoundService.postCompounds(fromValue);
    }
    service.subscribe(({ ok, message, result }: any) => {
      this.loading = false;

      if (ok) {
        this.rightDrawer.close(result);
        message = this.languageService.lang == 'en' ? 'Saved successfully !' : 'تم الحفظ بنجاح !';
        this.snackBar.open(message, 'OK', {
          duration: 3000,
          horizontalPosition: 'start',
          panelClass: ['bg-success']
        })
      } else {
        this.snackBar.open(message, 'ERROR', {
          duration: 3000,
          horizontalPosition: 'start',
          panelClass: ['bg-danger']
        })
      }
    })
  }
}
