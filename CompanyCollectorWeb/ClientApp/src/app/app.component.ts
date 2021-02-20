import { Component, OnInit } from '@angular/core';
import { CompanyViewModel } from './models/company-view-model';
import { SignalRService } from './services/signal-r.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit{
  signalList: CompanyViewModel[] = [];

  constructor(private signalRService: SignalRService){}

  ngOnInit(){
    this.signalRService.signalReceived.subscribe((company: CompanyViewModel) => {
      this.signalList.push(company);
    });
  }
}
