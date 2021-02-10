import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { CsvExportServiceService } from '../csv-export-service.service';

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

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {;
    this.url = baseUrl + 'CompanyCollector';
    this.httpClient = http;
  }

  ngOnInit(): void {
  }

  getCompanies(): void
  {
    this.statusText = 'Loading...';
    this.httpClient.get<string[]>(this.url).subscribe(result => {
        this.companies = result;
        CsvExportServiceService.exportToCsv('WikSoft_Companies.csv', result);
        this.statusText = '';
      }, error => console.error(error));
  }
}