import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import * as signalR from '@microsoft/signalr';  

@Component({
  selector: 'data-export-list',
  templateUrl: './dataexportlist.component.html',
})
export class DataExportListComponent {
    constructor(private http: HttpClient) {
    }

    private dataExports: DataExport[];

    public updateExport(dataExport: DataExport){
        console.log(dataExport);
        this.dataExports.unshift(dataExport);
    }

    public createNewExport (){
        this.http.post('http://localhost:42779/DataExport', null).subscribe(data => {
            console.log('click response');
        })
    }

    ngOnInit(): void {  
        this.http.get<DataExport[]>('http://localhost:42779/DataExport')
            .subscribe((data: DataExport[]) => this.dataExports = data);
      
        const connection = new signalR.HubConnectionBuilder()  
          .configureLogging(signalR.LogLevel.Information)  
          .withUrl('http://localhost:42779/exportHub')  
          .build();  
      
        connection.start().then(function () {  
          console.log('SignalR Connected!');  
        }).catch(function (err) {  
          return console.error(err.toString());  
        });  
      
        connection.on("SendExportUpdated", this.updateExport.bind(this));  
    }    
}
