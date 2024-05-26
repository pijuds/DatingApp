import { Component, Input, OnInit } from '@angular/core';
import { Toast, ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.scss']
})
export class MemberCardComponent implements OnInit {

  @Input() member: Member | undefined;

  constructor(private memberservice:MembersService,private toastr:ToastrService)
  {

  }

  ngOnInit(): void {
    
  }

  addLike(member: Member)
  {
    this.memberservice.addLike(member.userName ).subscribe({
      next:() => this.toastr.success('you have liked' + this.member?.knownAs)
    })
  }
}
