import { Component, ViewChild, OnInit, Input } from '@angular/core';

import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';

import { GridColumn } from './grid.model';
import { TradeEntry } from '../services/data.model'

@Component({
    selector: 'app-grid',
    templateUrl: './grid.component.html',
})
export class GridComponent implements OnInit {
    constructor() { }

    @Input() columns: GridColumn[];
    @Input() data: TradeEntry[];

    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;

    dataSource = new MatTableDataSource<TradeEntry>(this.data);
    columnNames: string[];

    ngOnInit() {
        this.columnNames = this.columns.map(x => x.name);

        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
    }

  
}
