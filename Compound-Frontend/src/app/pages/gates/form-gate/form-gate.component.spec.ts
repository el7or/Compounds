import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormGateComponent } from './form-gate.component';

describe('FormGateComponent', () => {
  let component: FormGateComponent;
  let fixture: ComponentFixture<FormGateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FormGateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FormGateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
