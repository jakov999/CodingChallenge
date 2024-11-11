import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CryptoService } from './crypto.service';

describe('CryptoService', () => {
  let service: CryptoService;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [CryptoService]
    });
    service = TestBed.inject(CryptoService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch prices for a specific currency', () => {
    const mockPrices = [
      { currency: 'bitcoin', price: 50000, dateReceived: new Date() },
      { currency: 'bitcoin', price: 51000, dateReceived: new Date() },
    ];

    service.getPricesByCurrency('bitcoin').subscribe((prices) => {
      expect(prices).toEqual(mockPrices);
    });

    const req = httpTestingController.expectOne(`${service['apiUrl']}/cryptoprices/bitcoin`);
    expect(req.request.method).toBe('GET');
    req.flush(mockPrices);
  });
});
