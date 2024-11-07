import { Component, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms'; // Import FormsModule
import { CryptoService } from '../../services/crypto.service';
import { CryptoPrice } from '../../models/crypto-price.model';
import { CommonModule, UpperCasePipe } from '@angular/common';
@Component({
  selector: 'app-specific-currency-prices',
  templateUrl: './specific-currency-prices.component.html',
  imports: [FormsModule, CommonModule, UpperCasePipe], // Add FormsModule here
  standalone: true
})
export class SpecificCurrencyPricesComponent implements OnDestroy {
  currency: string = 'bitcoin'; // Example default currency
  prices: CryptoPrice[] = [];
  private intervalId: any;

  constructor(private cryptoService: CryptoService) {}

  getPrices(): void {
    if (this.currency) {
      this.cryptoService.getPricesByCurrency(this.currency).subscribe({
        next: (data) => {
          this.prices = data;
        },
        error: (error) => console.error('Error fetching prices:', error)
      });
    }
  }

  setupAutoRefresh(): void {
    this.intervalId = setInterval(() => {
      this.getPrices();
    }, 60000); // Refresh every 60000 milliseconds (1 minute)
  }

  // Call setupAutoRefresh when the component initializes
  ngOnInit(): void {
    this.getPrices(); // Initial fetch
    this.setupAutoRefresh(); // Set up auto-refresh
  }

  ngOnDestroy(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId); // Clear interval on component destroy
    }
  }
}
