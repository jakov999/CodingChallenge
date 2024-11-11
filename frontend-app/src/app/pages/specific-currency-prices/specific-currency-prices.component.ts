import { Component, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms'; // Import FormsModule
import { CryptoService } from '../../services/crypto.service';
import { CryptoPrice } from '../../models/crypto-price.model';
import { CommonModule, UpperCasePipe } from '@angular/common';
import { ChartConfiguration, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-specific-currency-prices',
  templateUrl: './specific-currency-prices.component.html',
  imports: [FormsModule, CommonModule, UpperCasePipe, BaseChartDirective],
  standalone: true
})
export class SpecificCurrencyPricesComponent implements  OnDestroy {
  currency: string = 'bitcoin';
  prices: CryptoPrice[] = [];
  private intervalId: any;

  public barChartData: ChartConfiguration['data'] = {
    datasets: [
      {
        data: [],
        label: 'Price',
        backgroundColor: 'rgba(30, 136, 229, 0.3)',
        borderColor: '#1e88e5',
        fill: true,
        tension: 0.1,
      },
    ],
    labels: [],
  };
  public barChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    scales: {
      x: {},
      y: {
        beginAtZero: false,
      },
    },
  };
  public lineChartType: ChartType = 'line';

  constructor(private cryptoService: CryptoService) {}

  ngOnInit(): void {
    this.getPrices();
    this.setupAutoRefresh();
  }

  getPrices(): void {
    if (this.currency) {
      this.cryptoService.getPricesByCurrency(this.currency).subscribe({
        next: (data) => {
          this.prices = data;
          this.updateChart();
        },
        error: (error) => console.error('Error fetching prices:', error),
      });
    }
  }

  setupAutoRefresh(): void {
    this.intervalId = setInterval(() => {
      this.getPrices();
    }, 60000);
  }


  private updateChart(): void {
    this.barChartData.labels = this.prices.map((price) =>
      new Date(price.dateReceived).toLocaleString()
    );
    this.barChartData.datasets[0].data = this.prices.map((price) => price.price);
  }

  ngOnDestroy(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }
}
