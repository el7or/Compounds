import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormCompoundComponent } from './form-compound.component';

describe('FormCompoundComponent', () => {
  let component: FormCompoundComponent;
  let fixture: ComponentFixture<FormCompoundComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FormCompoundComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FormCompoundComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
