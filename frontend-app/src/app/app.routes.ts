import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { NewestPricesComponent } from './pages/newest-prices/newest-prices.component';
import { SpecificCurrencyPricesComponent } from './pages/specific-currency-prices/specific-currency-prices.component';
import { provideRouter } from '@angular/router';
export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'newest-prices', component: NewestPricesComponent },
  { path: 'currency-prices', component: SpecificCurrencyPricesComponent },
];