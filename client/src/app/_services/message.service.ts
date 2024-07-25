import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { getPaginatedHeaders, getPaginatedResult } from './paginationHelper';
import { Message } from '../_models/message';
import { HttpTransportType, HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { BehaviorSubject, take } from 'rxjs';
import { User } from '../_models/user';
import { ToastrService } from 'ngx-toastr';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl="https://localhost:5001/api/";
  hubUrl = 'https://localhost:5001/hubs/';
  private hubConnection?: HubConnection;
  private toastr = inject(ToastrService);
  private messageThreadSouce = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSouce.asObservable();

  constructor(private http:HttpClient) {


   }

   createHubConnection(user: User, otherUsername: string) {
    
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => user.token,
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      }).withAutomaticReconnect()
      .build();

      this.hubConnection
      .start()
      .catch(error => console.log('Error while starting connection: ' + error));

      this.hubConnection.on('ReceiveMessageThread', messages => {
        this.messageThreadSouce.next(messages);
      })

      this.hubConnection.on('UpdateGroup', (group:Group) => {
        if(group.connections.some(x=>x.username===otherUsername))
        {
          this.messageThread$.pipe(take(1)).subscribe({
            next:messages=>{
              messages.forEach(message=>{
                if(!message.dateRead)
                {
                  message.dateRead=new Date(Date.now());
                }
              })
              this.messageThreadSouce.next([...messages]);

            }
          })
        }
        ;
      })

      this.hubConnection.on('NewMessage', message => {
        this.messageThread$.pipe(take(1)).subscribe({
          next: messages => {
            this.messageThreadSouce.next([...messages, message])
          }
        })
      })

      
    }

    stopHubConnection() {
      if (this.hubConnection?.state === HubConnectionState.Connected) {
        this.hubConnection?.stop().catch(error => console.log('Error while stopping connection: ' + error));
      }
    }
   

   getMessages(pageNumber:number,pageSize:number,container:string)
   {
     let params=getPaginatedHeaders(pageNumber,pageSize);
     params=params.append('Container',container);

     return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
   }

   getMessageThread(username:string)
   {
      return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
   }

   //sendMessage(username:string,content:string)
   //{
   // return this.http.post<Message>(this.baseUrl + 'messages',{ recipientUsername: username, content })
   //}

   async sendMessage(username: string, content: string) {
    console.log("contenttt",content,"username",username);
    return this.hubConnection?.invoke('SendMessage', { recipientUsername: username, content })
      .catch(error => console.log(error));
  }


   deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
