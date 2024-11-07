import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewestPricesComponent } from './newest-prices.component';

describe('NewestPricesComponent', () => {
  let component: NewestPricesComponent;
  let fixture: ComponentFixture<NewestPricesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewestPricesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NewestPricesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
