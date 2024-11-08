import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CryptoService } from './services/crypto.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  prices: any[] = [];
  currency: string = 'bitcoin'; // Example: Default currency to fetch prices for
  title = 'frontend-app';
  constructor(private cryptoService: CryptoService) {}

  ngOnInit(): void {
    this.getPrices();
  }

  // Fetch the latest 10 prices for the specified currency
  getPrices(): void {
    this.cryptoService.getPricesByCurrency(this.currency)
      .subscribe({
        next: (data) => {
          console.log('API Response:', data); // Log the API response
          this.prices = data;
        },
        error: (error) => {
          console.error('Error fetching prices:', error);
        }
      });
  }
}
