import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'data-export-details',
  templateUrl: './dataexportdetails.component.html',
})
export class DataExportDetailsComponent {
    constructor(private http: HttpClient, private route: ActivatedRoute) {
    }

    private dataExport: DataExportDetails;

    ngOnInit(): void {  
      this.route.queryParams.subscribe(params => {
        this.http.get<DataExportDetails>('/DataExport/' + params['id'])
            .subscribe((data: DataExportDetails) => this.dataExport = data);
      });        
    }    
}