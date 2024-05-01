import { NgFor } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.scss']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm :NgForm | undefined;

  member: Member | undefined;

  user: User | null=null;
  
  constructor(private accountservice:AccountService,private memberservice:MembersService,private toastr:ToastrService){
   this.accountservice.currentUser$.pipe(take(1)).subscribe(
    {
      next:user=> this.user =user
    }
   )
  }
  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    if(!this.user) return;
    this.memberservice.getMember(this.user.username).subscribe({
      next:member=>this.member=member

    })
  }

  updateMember() {
    this.memberservice.updateMember(this.editForm?.value).subscribe({
      next: _ => {
        this.toastr.success('Profile updated successfully');
        this.editForm?.reset(this.member);
      }
    })
  }

}
