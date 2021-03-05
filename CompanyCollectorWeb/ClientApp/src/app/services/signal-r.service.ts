import { EventEmitter, Injectable } from "@angular/core";
import * as signalR from  "@aspnet/signalr";
import { CompanyViewModel } from "../models/company-view-model";

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: signalR.HubConnection;
  signalReceived = new EventEmitter<CompanyViewModel>();

  constructor() {
    this.buildConnection();
    this.startConnection();
   }

  public buildConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:44364/companyHub")
    .build();
  }

  public startConnection = () => {
    this.hubConnection
    .start()
    .then(() => {
      console.log("Connection started");
      this.registerSignalEvents();
    })
    .catch(err=> {
      console.log("Error while starting connection: " + err);

      setTimeout(function(){this.startConnection();}, 3000);
    })
  }

  private registerSignalEvents(){
    this.hubConnection.on("CompanyMessageReceived", (data: CompanyViewModel)=> {
      this.signalReceived.emit(data);
    })
  }
}
