import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';
import { Pagination } from '../_models/Pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.scss']
})
export class ListsComponent implements OnInit {
members: Member[] | undefined;
predicate='liked';
pageNumber=1;
pageSize=5;
pagination:Pagination | undefined;
  constructor(private member:MembersService)
  {


  }
  ngOnInit(): void {
    this.loadLikes()
  }

  loadLikes()
  {
    this.member.getLikes(this.predicate,this.pageNumber,this.pageSize).subscribe({
      next: response => {
        this.members=response.result;
        this.pagination=response.pagination;
      }
      
    })
  }

  pageChanged(event:any)
  {
    if (this.pageNumber !== event.page) {      
      this.pageNumber = event.page;
      this.loadLikes();
    }
  }


  

}
