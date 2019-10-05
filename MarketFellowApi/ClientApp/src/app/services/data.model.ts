export class MarketFellowQuery {
    marketProviders: MarketProvider[];
    tradingPairs: TradePair[];
    tradingEntries: TradeEntry[];
}

export class MarketProvider {
    id: number;
    name: string;
}

export class TradePair {
    id: number;
    name: string;
}

export class Filters {
    constructor(marketProvider: number, tradePair: number) {
        this.MarketProvider = marketProvider;
        this.TradePair = tradePair;
    }

    MarketProvider: number
    TradePair: number
}

import { GridColumn } from '../grid/grid.model'
export class TradeEntry {
    price: number;
    tradePair: string;
    time: Date;

    static getGridColumns(): GridColumn[] {
        return [
            { caption: 'Trade Pair', name: 'tradePair' },
            { caption: 'Price', name: 'price' },
            { caption: 'Time', name: 'time' },
        ];
    } 
}


