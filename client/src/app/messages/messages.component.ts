import { Component } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/Pagination';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss']
})
export class MessagesComponent {

  constructor(private message:MessageService)
  {

  }
messages?: Message[];
pagination?:Pagination;
container='Inbox';
pageNumber=1;
pageSize=5;

ngOnInit():void
{
  this.loadMessages();

}

loadMessages()
{
  this.message.getMessages(this.pageNumber,this.pageSize,this.container).subscribe({
    next:response=>{
      this.messages=response.result;
      console.log("messages",this.messages);
      this.pagination=response.pagination;
    }
  })
}

pageChanged(event:any)
{
  if (this.pageNumber !== event.page) {      
    this.pageNumber = event.page;
    this.loadMessages();
  }
}

}
