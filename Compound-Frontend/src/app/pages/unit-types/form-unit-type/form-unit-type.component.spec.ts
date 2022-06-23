import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormUnitTypeComponent } from './form-unit-type.component';

describe('FormUnitTypeComponent', () => {
  let component: FormUnitTypeComponent;
  let fixture: ComponentFixture<FormUnitTypeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FormUnitTypeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FormUnitTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
