import { Injectable, inject } from '@angular/core';
import { HttpTransportType, HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  //hubUrl = 'https://localhost:5001/hubs/';
  hubsUrl=environment.hubUrl;
  private hubConnection?: HubConnection;
 
  private toastr = inject(ToastrService);
  private onlinesUseSource=new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlinesUseSource.asObservable();

  constructor(private router: Router) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubsUrl + 'presence', {
        accessTokenFactory: () => user.token,
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch(error => console.log('Error while starting connection: ' + error));

    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => this.onlinesUseSource.next([...usernames, username])
      })
    });

    this.hubConnection.on('UserIsOffline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: usernames => this.onlinesUseSource.next([...usernames.filter(x => x !== username)])
      })
    });
    this.hubConnection.on('GetOnlineUsers', usernames => {
      try {
        this.onlinesUseSource.next(usernames);
        console.log('usernames', usernames);
      } catch (error) {
        console.error('Error handling GetOnlineUsers event:', error);
      }
    });

    this.hubConnection.on('NewMessageReceived',({username,knownAs})=>{
      console.log("known as",knownAs);
      this.toastr.info(`${knownAs} has sent you new Message ! click me to see it`)
      
        .onTap
        .pipe(take(1))
        .subscribe(() => this.router.navigateByUrl('/members/' + username + '?tab=Messages'))
    })
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection?.stop().catch(error => console.log('Error while stopping connection: ' + error));
    }
  }
}