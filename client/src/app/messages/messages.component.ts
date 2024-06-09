import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/Pagination';
import { MessageService } from '../_services/message.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss']
})
export class MessagesComponent implements OnInit {
  messages?: Message[];
  pagination?:Pagination;
  container='Unread';
  pageNumber=1;
  pageSize=5;
  loading=false;
  constructor(private message:MessageService)
  {

  }


ngOnInit():void
{
  this.loadMessages(); 

}

loadMessages()
{
  this.loading=true;
  this.message.getMessages(this.pageNumber,this.pageSize,this.container).subscribe({
    next:response=>{
      this.messages=response.result;
      console.log("messages",this.messages);
      this.pagination=response.pagination;
      this.loading=false;
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

deleteMessage(id: number) {
  this.message.deleteMessage(id).subscribe({
    next: _ => this.messages?.splice(this.messages.findIndex(m => m.id === id), 1)
  })
}

}
