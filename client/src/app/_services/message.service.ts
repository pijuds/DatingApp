import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { getPaginatedHeaders, getPaginatedResult } from './paginationHelper';
import { Message } from '../_models/message';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl="https://localhost:5001/api/";

  constructor(private http:HttpClient) {


   }
   

   getMessages(pageNumber:number,pageSize:number,container:string)
   {
     let params=getPaginatedHeaders(pageNumber,pageSize);
     params=params.append('Container',container);

     return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
   }
}
