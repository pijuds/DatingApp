import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl="https://localhost:5001/api/";
  members:Member[]=[];
  //baseUrl=environment.apiUrl;

  constructor(private http:HttpClient) {


   }
   getMembers()
   {
    if(this.members.length>0) return of (this.members);
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members=>{
        this.members=members
        return members;
      })
    )
    
   }

   getMember(username :string)
   {
    const member=this.members.find(x=>x.userName==username)
    return this.http.get<Member>(this.baseUrl + 'users/' +username )
   }

   updateMember(member:Member)
   {
     return this.http.put(this.baseUrl +'users',member).pipe(
      map(()=>{
        const index=this.members.indexOf(member);
        console.log("index",index);
        this.members[index]={...this.members[index],...member}

        console.log("members update data",this.members[index]);
      })
     );
   }

  
}
