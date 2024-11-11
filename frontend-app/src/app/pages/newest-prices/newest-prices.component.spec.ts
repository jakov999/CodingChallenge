import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { NewestPricesComponent } from './newest-prices.component';
import { CryptoService } from '../../services/crypto.service';
import { CryptoPrice } from '../../models/crypto-price.model';

describe('NewestPricesComponent', () => {
  let component: NewestPricesComponent;
  let fixture: ComponentFixture<NewestPricesComponent>;
  let mockCryptoService: jasmine.SpyObj<CryptoService>;

  beforeEach(async () => {
    const cryptoServiceSpy = jasmine.createSpyObj('CryptoService', ['getLatestPrices']);

    await TestBed.configureTestingModule({
      imports: [NewestPricesComponent],
      providers: [{ provide: CryptoService, useValue: cryptoServiceSpy }]
    }).compileComponents();

    fixture = TestBed.createComponent(NewestPricesComponent);
    component = fixture.componentInstance;
    mockCryptoService = TestBed.inject(CryptoService) as jasmine.SpyObj<CryptoService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch the latest prices on initialization', () => {
    const mockPrices: CryptoPrice[] = [
      { currency: 'bitcoin', price: 50000, dateReceived: new Date() },
      { currency: 'ethereum', price: 1500, dateReceived: new Date() }
    ];


    mockCryptoService.getLatestPrices.and.returnValue(of(mockPrices));


    component.ngOnInit();


    expect(component.prices).toEqual(mockPrices);
    expect(mockCryptoService.getLatestPrices).toHaveBeenCalled();
  });

  it('should display "No prices available" when prices array is empty', () => {
    mockCryptoService.getLatestPrices.and.returnValue(of([]));

    component.ngOnInit();
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('No prices available');
  });
});
