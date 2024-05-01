import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.scss']
})
export class MemberListComponent {
  //members: Member[]=[];
  members$:Observable<Member[]> | undefined;

  constructor(private member:MembersService)
  {

  }

  ngOnInit(): void {

    this.members$=this.member.getMembers();
  }

  

  //loadMembers()
  ///{
    //console.log("data load");
    //this.member.getMembers().subscribe(
     // {
        //next:members=>this.members =members
      //}
    //)
  //}

}
