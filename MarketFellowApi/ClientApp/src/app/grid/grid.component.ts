import { Component, ViewChild, OnInit, Input } from '@angular/core';

import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';

import { GridColumn } from './grid.model';

@Component({
    selector: 'app-grid',
    templateUrl: './grid.component.html',
})
export class GridComponent implements OnInit {
    constructor() { }

    @Input() columns: GridColumn[];
    @Input() data: any[];

    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;

    dataSource = new MatTableDataSource(this.data);

    ngOnInit() {
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
    }

  
}
