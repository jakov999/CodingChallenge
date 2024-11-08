import { TestBed, ComponentFixture } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { SpecificCurrencyPricesComponent } from './specific-currency-prices.component';
import { CryptoService } from '../../services/crypto.service';
import { of } from 'rxjs';
import { CryptoPrice } from '../../models/crypto-price.model';

describe('SpecificCurrencyPricesComponent', () => {
  let component: SpecificCurrencyPricesComponent;
  let fixture: ComponentFixture<SpecificCurrencyPricesComponent>;
  let mockCryptoService: jasmine.SpyObj<CryptoService>;

  beforeEach(async () => {
    const cryptoServiceSpy = jasmine.createSpyObj('CryptoService', ['getPricesByCurrency']);

    await TestBed.configureTestingModule({
      imports: [FormsModule, SpecificCurrencyPricesComponent],
      providers: [{ provide: CryptoService, useValue: cryptoServiceSpy }]
    }).compileComponents();

    fixture = TestBed.createComponent(SpecificCurrencyPricesComponent);
    component = fixture.componentInstance;
    mockCryptoService = TestBed.inject(CryptoService) as jasmine.SpyObj<CryptoService>;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch prices on initialization', () => {
    const mockPrices: CryptoPrice[] = [
      { currency: 'bitcoin', price: 50000, dateReceived: new Date() },
      { currency: 'bitcoin', price: 51000, dateReceived: new Date() }
    ];

    mockCryptoService.getPricesByCurrency.and.returnValue(of(mockPrices));

    component.getPrices();
    expect(component.prices).toEqual(mockPrices);
    expect(mockCryptoService.getPricesByCurrency).toHaveBeenCalledWith('bitcoin');
  });
});
