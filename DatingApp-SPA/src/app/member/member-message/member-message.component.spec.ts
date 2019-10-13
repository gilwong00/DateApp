import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MemberMessageComponent } from './member-message.component';

describe('MemberMessageComponent', () => {
  let component: MemberMessageComponent;
  let fixture: ComponentFixture<MemberMessageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MemberMessageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberMessageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
