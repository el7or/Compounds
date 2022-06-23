import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdsFormComponent } from './ads-form.component';

describe('AdsFormComponent', () => {
  let component: AdsFormComponent;
  let fixture: ComponentFixture<AdsFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdsFormComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AdsFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
