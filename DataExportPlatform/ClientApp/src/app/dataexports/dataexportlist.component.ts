import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'data-export-list',
  templateUrl: './dataexportlist.component.html',
})
export class DataExportListComponent {
    constructor(private http: HttpClient) {
        this.http.get<DataExport[]>('http://localhost:42779/DataExport')
            .subscribe((data: DataExport[]) => this.dataExports = data);
    }

    private dataExports;
}
