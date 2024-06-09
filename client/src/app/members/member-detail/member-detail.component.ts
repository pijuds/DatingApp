import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.scss']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs',{static:true}) memberTabs?:TabsetComponent;

  member:Member={} as Member;
  activeTab?:TabDirective;
  messages:Message[]=[];

  constructor(private memberservice:MembersService,private route:ActivatedRoute,
    private messageservice:MessageService){}
  ngOnInit(): void {
    this.route.data.subscribe({
      next:data=>this.member=data['member']
    })

    this.route.queryParams.subscribe({
      next:params=>{
        params['tab']  && this.SelectTab(params['tab'])
      }
    })
  }

  SelectTab(heading:string)
  {
    if(this.memberTabs)
      {
        this.memberTabs.tabs.find(x=>x.heading===heading)!.active=true;
      }

  }

  onTabActivated(data:TabDirective)
  {
    this.activeTab=data;
    if(this.activeTab.heading==='Messages')
      {
        this.loadMessages();

      }

  }

  loadMessages()
  {
    if(this.member)
      {
        this.messageservice.getMessageThread(this.member.userName).subscribe({
          next:messages=>this.messages =messages
        })
      }
  }

//loadMember()
 // {
  //  var username=this.route.snapshot.paramMap.get('username');
    //if(!username) return

    //this.memberservice.getMember(username).subscribe({
     // next:member=>this.member=member
    //})
 // }

}
