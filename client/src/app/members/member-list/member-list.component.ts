import { Component } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Pagination } from 'src/app/_models/Pagination';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.scss']
})
export class MemberListComponent {
  members: Member[]=[];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  pageNumber=1;
  pageSize=5;
 
  genderList=[{ value: 'male', display: 'Males' }, { value: 'female', display: 'Females' }];
 // members$:Observable<Member[]> | undefined;

  constructor(private member:MembersService,private accountService:AccountService)
  {
   this.userParams=this.member.getUserParams();
  }

  ngOnInit(): void {

    this.loadMembers();

    //this.members$=this.member.getMembers();
  }

  loadMembers()
  {
    if(this.userParams)
    {
      this.member.setUserParams(this.userParams);
      this.member.getMembers(this.userParams).subscribe({
      
        next:response=>{
          if(response.result && response.pagination)
            {
              this.members=response.result,
              this.pagination=response.pagination
            }
        }
      })

    }
    
  }

  pageChanged(event:any)
  {
    if (this.userParams && this.userParams?.pageNumber !== event.page) {      
      this.userParams.pageNumber = event.page;
      this.member.setUserParams(this.userParams);
      this.loadMembers();
    }
  }

  resetFilters() {
    
    this.userParams = this.member.resetUserParams();
    this.loadMembers();
    
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
