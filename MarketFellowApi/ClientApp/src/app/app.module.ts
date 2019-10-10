import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { ApolloModule, APOLLO_OPTIONS, Apollo } from "apollo-angular";
import { HttpLinkModule, HttpLink } from "apollo-angular-link-http";
import { WebSocketLink } from 'apollo-link-ws';
import { InMemoryCache } from "apollo-cache-inmemory";
import { MatSortModule } from '@angular/material/sort';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { AppRoutingModule } from './/app-routing.module';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { GridComponent } from './grid/grid.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatGridListModule } from '@angular/material/grid-list';
import { split } from 'apollo-link';
import { getMainDefinition } from 'apollo-utilities';
import { OperationDefinitionNode } from 'graphql';
import { SubscriptionClient } from 'subscriptions-transport-ws';

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        GridComponent,
    ],
    imports: [
        BrowserAnimationsModule,
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        AppRoutingModule,
        HttpClientModule,
        FormsModule,
        MatSortModule,
        MatToolbarModule,
        MatSelectModule,
        MatCardModule,
        MatTableModule,
        MatPaginatorModule,
        MatGridListModule,
        HttpLinkModule,
        ApolloModule
    ],
    //providers: [{
    //    provide: APOLLO_OPTIONS,
    //    useFactory: (httpLink: HttpLink) => {
    //        return {
    //            cache: new InMemoryCache(),
    //            link: httpLink.create({
    //                uri: "http://localhost:55241/graphql"
    //            })
    //        }
    //    },
    //    deps: [HttpLink]
    //}],
    bootstrap: [AppComponent]
})
export class AppModule {
    constructor(
        apollo: Apollo,
        httpLink: HttpLink
    ) {
        // Create an http link:
        const http = httpLink.create({
            uri: 'http://localhost:55241/graphql'
        });

        // Create a WebSocket link:
        const ws = new WebSocketLink({
            uri: `ws://localhost:55241/graphql`,
            options: {
                reconnect: true,
                lazy: true,
                inactivityTimeout: 50000, timeout: 50000
            }
        });

        // using the ability to split links, you can send data to each link
        // depending on what kind of operation is being sent
        const link = split(
            // split based on operation type
            ({ query }) => {
                var node = getMainDefinition(query);
                return node.kind === 'OperationDefinition' && node.operation === 'subscription';
            },
            ws,
            http,
        );

        apollo.create({ cache: new InMemoryCache(), link: link });
    }
}

