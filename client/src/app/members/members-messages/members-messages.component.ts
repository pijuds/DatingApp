import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-members-messages',
  templateUrl: './members-messages.component.html',
  styleUrls: ['./members-messages.component.scss']
})
export class MembersMessagesComponent implements OnInit {
  @Input() username?:string;
 // @Input() messages :Message[]=[];
  messageContent='';
  @ViewChild('messageForm') messageForm?:NgForm;

  constructor(public messageservice:MessageService)
  {

  }

  

  ngOnInit(): void {
   // this.loadMessages();
  }

  sendMessage()
  {
    if(!this.username) return;
    this.messageservice.sendMessage(this.username,this.messageContent).then(()=>{
    
        //this.messages.push(mesaage);
        this.messageForm?.reset();


      })

    
  }

}
