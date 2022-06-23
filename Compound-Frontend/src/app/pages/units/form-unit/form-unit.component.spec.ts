import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormUnitComponent } from './form-unit.component';

describe('FormUnitComponent', () => {
  let component: FormUnitComponent;
  let fixture: ComponentFixture<FormUnitComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FormUnitComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FormUnitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
