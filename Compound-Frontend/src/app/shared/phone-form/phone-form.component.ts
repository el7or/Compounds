import { Component, Input, OnInit } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormGroup, NG_VALIDATORS, NG_VALUE_ACCESSOR, ValidationErrors, Validator, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { map } from 'rxjs/operators';
import { ICountry, PhoneFormService } from './phone-form.service';

@Component({
  selector: 'app-phone-form',
  templateUrl: './phone-form.component.html',
  styleUrls: ['./phone-form.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: PhoneFormComponent,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: PhoneFormComponent,
    }
  ]
})
export class PhoneFormComponent implements ControlValueAccessor, Validator, OnInit {
  @Input() label: string = '';
  @Input() hint: string = '';
  @Input() error: string = '';
  @Input() defaultCode: string = '';
  private mobileReg = '^((\\+91-?)|0)?[0-9]{3,11}$';
  codeNumberForm: FormGroup = new FormGroup({
    code: new FormControl(null, [Validators.required]),
    number: new FormControl(null, [Validators.required, Validators.pattern(this.mobileReg)])
  });
  selectedCountry: ICountry | undefined;
  countries: ICountry[] = [];
  touched: any;

  onTouched = () => { };
  onChangeSubs: Subscription[] = [];

  constructor(private phoneFormService: PhoneFormService) { }

  ngOnDestroy() {
    for (let sub of this.onChangeSubs) {
      sub.unsubscribe();
    }
  }
  ngOnInit(): void {
    //get countries data
    this.phoneFormService.getAllCountries().subscribe(countries => {
      this.countries = countries;
      // this to log duplicated
      // this.countries.forEach(v => {
      //   if (this.countries.findIndex(i => v.dialCode == i.dialCode) != -1)
      //     console.log({ v });
      // })
      //sort countries data
      this.countries.sort((a, b) => (a.name.en > b.name.en) ? 1 : ((b.name.en > a.name.en) ? -1 : 0));
      this.codeNumberForm.get('code')?.valueChanges.subscribe(code =>
        //get Country object to you the emoji flags
        this.selectedCountry = this.countries.find(i => i.dialCode == code)
      );
      //set default Code
      this.codeNumberForm.get('code')?.setValue(this.countries.find(i => i.code == this.defaultCode)?.dialCode);
    });
  }
  writeValue(value: number) {
    if (value) {
      this.codeNumberForm.setValue({
        code: Number(value.toString().slice(0, 1)),
        number: Number(value.toString().slice(2, value.toString().length))
      }, { emitEvent: false });
    }
  }
  validate(control: AbstractControl): ValidationErrors | null {
    if (this.codeNumberForm.valid) {
      return null;
    }
    let errors: any = {};
    errors = this.addControlErrors(errors, "number");
    errors = this.addControlErrors(errors, "code");
    return errors;
  }
  addControlErrors(allErrors: any, controlName: string) {
    const errors = { ...allErrors };
    const controlErrors = this.codeNumberForm.controls[controlName]?.errors;
    if (controlErrors) {
      errors[controlName] = controlErrors;
    }
    return errors;
  }
  onChange = (data: any) => this.writeValue(data);
  markAsTouched() {
    if (!this.touched) {
      this.onTouched();
      this.touched = true;
    }
  }
  registerOnChange(onChange: any): void {
    const sub = this.codeNumberForm.valueChanges.pipe(map(i => {
      // Overwrite controls data
      if (!i.code || !i.number)
        return null;
      return Number(`${i.code}${i.number}`);
    })).subscribe(onChange);
    this.onChangeSubs.push(sub);
  }
  registerOnTouched(onTouched: any): void {
    this.onTouched = onTouched;
  }
  setDisabledState?(disabled: boolean): void {
    if (disabled)
      this.codeNumberForm.disable();
    else
      this.codeNumberForm.enable();
  }
}
