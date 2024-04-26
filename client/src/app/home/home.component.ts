
import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  registerMode=false;
  users:any;

  constructor()
  {

  }

  ngOnInit(): void {

    
    
  }

  registerToggle()
  {
    this.registerMode= !this.registerMode;
  }

  
  register()
  {

  }

  cancelRegisterMode(event:boolean)
  {
    this.registerMode=event;
  }


}
