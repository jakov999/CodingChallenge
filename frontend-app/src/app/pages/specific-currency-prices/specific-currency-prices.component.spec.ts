import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpecificCurrencyPricesComponent } from './specific-currency-prices.component';

describe('SpecificCurrencyPricesComponent', () => {
  let component: SpecificCurrencyPricesComponent;
  let fixture: ComponentFixture<SpecificCurrencyPricesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SpecificCurrencyPricesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SpecificCurrencyPricesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
