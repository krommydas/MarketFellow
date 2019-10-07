export class MarketFellowSubscription {
    tradyEntry: TradeEntry;
}

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
    id: string;
    name: string;
}

export class Filters {
    constructor(marketProvider: number, tradePair: string) {
        this.MarketProvider = marketProvider;
        this.TradePair = tradePair;
    }

    MarketProvider: number
    TradePair: string
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


