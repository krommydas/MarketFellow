import { Component, ViewChild, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { DataService } from '../services/data.service';
import { MarketProvider, TradePair, Filters, TradeEntry } from '../services/data.model';
import { GridColumn } from '../grid/grid.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles:['mat-card { margin-bottom:10px;}']
})
export class HomeComponent implements OnInit {
    constructor(private router: Router, private currentRoute: ActivatedRoute, private dataService: DataService) { }

    marketProviders: MarketProvider[];
    tradePairs: TradePair[];
    filters: Filters;
    tradeEntriesFeed: Observable<TradeEntry[]>;

    tradeEntryGridColumns: GridColumn[];

    ngOnInit() {
        this.dataService.getMarketProviders().subscribe(result => this.marketProviders = result);

        this.tradeEntryGridColumns = TradeEntry.getGridColumns();

        var routeParams = this.currentRoute.snapshot.paramMap;
        var selectedProvider = +routeParams.get('provider');
        var selectedPair = routeParams.get('tradingPair');

        if (selectedProvider)
            this.dataService.getTradePairs(selectedProvider).subscribe(result => this.tradePairs = result);

        this.filters = new Filters(selectedProvider, selectedPair);

        if (selectedProvider && selectedPair)
            this.tradeEntriesFeed = this.dataService.getTradeEntriesFeed(this.filters);
    }

    onProviderSelected(selectedProvider: number) {
        this.router.navigate(['/home', { provider: selectedProvider}]);
    }

    onTradingPairSelected(selectedPair: number) {
        this.router.navigate(['/home', { provider: this.filters.MarketProvider, tradingPair: selectedPair }]);
    }
}
