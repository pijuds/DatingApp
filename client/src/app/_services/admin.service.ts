import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../_models/user';


@Injectable({
  providedIn: 'root'
})
export class AdminService {

  baseUrl="https://localhost:5001/api/";

  constructor(private http: HttpClient) { }

  getUsersWithRoles() {
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(userName:string,roles: string[])
  {
    return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/' 
      + userName  + '?roles=' + roles, {});
  }
}