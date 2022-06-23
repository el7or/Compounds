import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReplyLayoutComponent } from './reply-layout.component';

describe('ReplyLayoutComponent', () => {
  let component: ReplyLayoutComponent;
  let fixture: ComponentFixture<ReplyLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReplyLayoutComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReplyLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
