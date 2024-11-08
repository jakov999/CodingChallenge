import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CryptoPrice } from '../models/crypto-price.model';

@Injectable({
  providedIn: 'root'
})
export class CryptoService {
  private apiUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  // Fetch the latest 10 prices for a specific currency
  getPricesByCurrency(currency: string): Observable<CryptoPrice[]> {
    return this.http.get<any>(`${this.apiUrl}/cryptoprices/${currency}`);
  }

  // Fetch the latest prices for all currencies
  getLatestPrices(): Observable<CryptoPrice[]> {
    return this.http.get<any>(`${this.apiUrl}/cryptoprices/latest`);
  }
}
