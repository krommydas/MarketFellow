import { Injectable, Query } from '@angular/core';
import { Observable } from 'rxjs';
import { Apollo, QueryRef  } from 'apollo-angular';
import gql  from 'graphql-tag';
import { MarketProvider, TradePair, Filters, TradeEntry, MarketFellowQuery, MarketFellowSubscription } from './data.model';
import { map } from 'rxjs/operators';
import { of } from 'zen-observable';

@Injectable({ providedIn: 'root' })
export class DataService {
    constructor(private apollo: Apollo) { }

    getMarketProviders(): Observable<MarketProvider[]> {
        return this.apollo.watchQuery<MarketFellowQuery>({
            query: gql`query marketProviders { marketProviders { id name }}`
        }).valueChanges.pipe(map(result => result.data.marketProviders));
    }

    getTradePairs(provider: number): Observable<TradePair[]> {
        return this.apollo.watchQuery<MarketFellowQuery>({
            query: gql`query tradingPairs { tradingPairs(marketProvider: ${provider}) { id name }}`
        }).valueChanges.pipe(map(result => result.data.tradingPairs));
    }

    getTradeEntries(filters: Filters): Observable<TradeEntry[]> {
        return this.getTradeEntriesQuery(filters).valueChanges.pipe(map(result => result.data.tradingEntries));
    }

    getTradeEntriesFeed(filters: Filters): Observable<TradeEntry[]> {
        var query = this.getTradeEntriesQuery(filters);

        query.subscribeToMore({
            document: gql`subscription tradingEntriesFeed {
                                              tradeEntry(marketProvider: ${filters.MarketProvider}, tradingPair: "${filters.TradePair}")
                                              { price time tradePair }}`,
            updateQuery: (prev, { subscriptionData }) => {
                if (!subscriptionData.data) {
                    return prev;
                }

                const newItem = subscriptionData.data.tradeEntry;

                prev.tradingEntries.push(newItem);

                return prev;
            }
        });

        return query.valueChanges.pipe(map(result => result.data.tradingEntries));
    }

    private getTradeEntriesQuery(filters: Filters): QueryRef<MarketFellowQuery> {
        return this.apollo.watchQuery<MarketFellowQuery>({
            query: gql`query tradingEntries {
                                              tradingEntries(marketProvider: ${filters.MarketProvider}, tradingPair: "${filters.TradePair}")
                                            { price time tradePair }}`
        });
    }
}
