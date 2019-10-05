import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { HomeComponent } from './home/home.component';

@NgModule({
  imports: [
        CommonModule,
        RouterModule.forRoot([
            { path: 'home', component: HomeComponent, pathMatch: 'full' },
          ])
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
