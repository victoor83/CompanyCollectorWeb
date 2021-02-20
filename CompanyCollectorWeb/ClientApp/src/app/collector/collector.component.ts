import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { CsvExportServiceService } from '../csv-export-service.service';
import { CompanyViewModel } from '../models/company-view-model';
import { SignalRService } from '../services/signal-r.service';

@Component({
  selector: 'app-collector',
  templateUrl: './collector.component.html',
  styleUrls: ['./collector.component.css']
})
export class CollectorComponent implements OnInit {
  public companies: string[];
  public statusText: string;
  private url: string;
  private httpClient: HttpClient;
  signalList: CompanyViewModel[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private signalRService: SignalRService) {
    this.url = baseUrl + 'CompanyCollector';
    this.httpClient = http;
  }

  ngOnInit(){
    this.signalRService.signalReceived.subscribe((company: CompanyViewModel) => {
      this.signalList.push(company);
    });
  }

  getCompanies(): void
  {
    this.statusText = 'Loading with SignalR...';
    this.signalList = [];
    this.httpClient.get<string[]>(this.url).subscribe(result => {
        CsvExportServiceService.exportToCsv('WikSoft_Companies.csv', result);
        this.statusText = '';
      }, error => console.error(error));
  }
}
