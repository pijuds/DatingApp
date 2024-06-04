import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/Pagination';
import { UserParams } from '../_models/userParams';
import { User } from '../_models/user';
import { AccountService } from './account.service';
import { getPaginatedHeaders, getPaginatedResult } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl="https://localhost:5001/api/";
  members:Member[]=[];
  user?: User | null;
  //paginatedResult:PaginatedResult<Member[]>=new PaginatedResult<Member[]>;
  userParams: UserParams | undefined;
  memberCache=new Map();
  //baseUrl=environment.apiUrl;

  constructor(private http:HttpClient,private accountService:AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user)
          this.userParams = new UserParams(user);
          this.user = user;
      }
    })

   }

   getUserParams() {
    return this.userParams;
  }

  setUserParams(userParams: UserParams) {
    this.userParams = userParams;
  }
  
   getMembers(userParams:UserParams)
   {
    const response=this.memberCache.get(Object.values(userParams).join('-'));
    console.log("response",response);

    let params=getPaginatedHeaders(userParams.pageNumber,userParams.pageSize);
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
    

   
    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    )
  
    //if(this.members.length>0) return of (this.members);
    
    //.pipe(
    //  map(members=>{
      //  this.members=members
       // return members;
      //})
    //)
    
   }

   
   getMember(username :string)
   {
    const member=[...this.memberCache.values()].reduce
    ((arr,elem)=>arr.concat(elem.result), [])
    .find((member:Member)=>member.userName===username);

    if(member) return of(member);
   // const member=this.members.find(x=>x.userName==username)
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

   setMainPhoto(PhotoId:number)
   {
     return this.http.put (this.baseUrl + 'users/set-main-photo/' +PhotoId,{});
   }

   deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  resetUserParams() {
    if (this.user) {
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {})
  }

  getLikes(predicate: string,pageNumber:number,pageSize:number) {
    console.log('predicate',predicate);
    let params=getPaginatedHeaders(pageNumber,pageSize);
    params= params.append('predicate',predicate);
    console.log('params',params)
    

    return getPaginatedResult<Member[]>(this.baseUrl + 'likes',params,this.http);
  }
  
}
