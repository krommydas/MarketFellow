import { Component, ViewChild, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { DataService } from '../services/data.service';
import { MarketProvider, TradePair, Filters, TradeEntry } from '../services/data.model';
import { GridColumn } from '../grid/grid.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
    constructor(private router: Router, private currentRoute: ActivatedRoute, private dataService: DataService) { }

    marketProviders: MarketProvider[];
    tradePairs: TradePair[];
    filters: Filters;

    tradeEntryGridColumns = TradeEntry.getGridColumns();

    ngOnInit() {
        this.dataService.getMarketProviders().subscribe(result => this.marketProviders = result);

        var routeParams = this.currentRoute.snapshot.paramMap;
        this.filters = new Filters(+routeParams.get('provider'), +routeParams.get('tradingPair'));
    }

    onProviderSelected(selectedProvider: number) {
        this.router.navigate(['/home', { provider: selectedProvider}]);
    }

    onTradingPairSelected(selectedPair: number) {
        this.router.navigate(['/home', { provider: selectedPair, tradingPair: selectedPair }]);
    }
}
