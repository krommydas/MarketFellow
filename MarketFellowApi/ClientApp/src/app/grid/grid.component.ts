import { Component, ViewChild, OnInit, Input, ViewEncapsulation } from '@angular/core';

import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';

import { GridColumn } from './grid.model';
import { TradeEntry } from '../services/data.model'
import { Observable } from 'rxjs';

@Component({
    selector: 'app-grid',
    templateUrl: './grid.component.html',
    styleUrls: ['./grid.component.css'],
    encapsulation: ViewEncapsulation.None
})
export class GridComponent<GridItem> implements OnInit {
    constructor() { }

    @Input() columns: GridColumn[];
    @Input() data: Observable<GridItem[]>;

    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;

    dataSource = new MatTableDataSource<GridItem>([]);
    columnNames: string[];

    ngOnInit() {
        this.columnNames = this.columns.map(x => x.name);

        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;

        if (this.data)
            this.data.subscribe(x => this.dataSource.data = x);
    }

  
}
