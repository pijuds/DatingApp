import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl="https://localhost:5001/api/";
  //baseUrl=environment.apiUrl;
  private currentUserSource=new BehaviorSubject<User | null>(null);
  currentUser$=this.currentUserSource.asObservable();
  constructor(private http:HttpClient) { }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;

        if (user) {
          this.setCurrentUser(user)
          
           //this.currentUserSource.next(user);
        }

        return user;
      })
    );
  }
  setCurrentUser(user:User)
  {
     localStorage.setItem('user', JSON.stringify(user));
     this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    
  }

  registerUser(model:any)
  {

    console.log("modeldata",model)
    return this.http.post<User>(this.baseUrl + 'account/register',model).pipe(map(
      user=>{
        if(user){
          this.setCurrentUser(user)
          //this.currentUserSource.next(user);
        }
        return user;

      }
     
    ))
  }
}
