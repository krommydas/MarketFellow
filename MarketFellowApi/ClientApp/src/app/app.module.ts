import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { ApolloModule, APOLLO_OPTIONS, Apollo } from "apollo-angular";
import { HttpLinkModule, HttpLink } from "apollo-angular-link-http";
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
import { DataService } from './services/data.service';

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
      HttpLinkModule,
      ApolloModule
  ],
    providers: [{
        provide: APOLLO_OPTIONS,
        useFactory: (httpLink: HttpLink) => {
            return {
                cache: new InMemoryCache(),
                link: httpLink.create({
                    uri: "http://localhost:55241/graphql"
                })
            }
        },
        deps: [HttpLink]
    }],
  bootstrap: [AppComponent]
})
export class AppModule {
    //constructor(private apollo: Apollo, private httpLink: HttpLink) {
    //    apollo.create({
    //        link: httpLink.create({ uri: 'https://localhost:55241/graphql' }),
    //        cache: new InMemoryCache()
    //    });
    }

