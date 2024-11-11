import { Component, OnDestroy, OnInit } from '@angular/core';
import { CryptoService } from '../../services/crypto.service';
import { CryptoPrice } from '../../models/crypto-price.model';
import { CommonModule, UpperCasePipe } from '@angular/common';
@Component({
  selector: 'app-newest-prices',
  templateUrl: './newest-prices.component.html',
  imports: [CommonModule, UpperCasePipe],
  standalone: true 
})
export class NewestPricesComponent implements OnInit, OnDestroy {
  prices: CryptoPrice[] = [];
  private intervalId: any;

  constructor(private cryptoService: CryptoService) {}

  ngOnInit(): void {
    this.getLatestPrices(); // Initial fetch
    this.setupAutoRefresh(); // Set up auto-refresh
  }

  getLatestPrices(): void {
    this.cryptoService.getLatestPrices().subscribe({
      next: (data) => {
        this.prices = data;
      },
      error: (error) => console.error('Error fetching prices:', error)
    });
  }

  setupAutoRefresh(): void {
    this.intervalId = setInterval(() => {
      this.getLatestPrices();
    }, 60000); // Refresh every 60000 milliseconds (1 minute)
  }

  ngOnDestroy(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId); // Clear interval on component destroy
    }
  }
}