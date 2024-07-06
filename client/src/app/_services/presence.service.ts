import { Injectable, inject } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl = 'https://localhost:50001/hubs/';
  private hubConnection?: HubConnection;
  private toastr=inject(ToastrService);

  constructor() { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

      this.hubConnection
      .start()
      .catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => {
      this.toastr.info(user +"is connected")
      
      })

      this.hubConnection.on('UserIsOffline', username => {
        this.toastr.info(user +"is not connected")
      })
    }

    stopHubConnection() {
      if(this.hubConnection?.state===HubConnectionState.Connected)
      {
           this.hubConnection?.stop().catch(error => console.log(error));
      }
    }
}
