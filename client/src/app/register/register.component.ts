import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  @Input() usersFormComponent:any;
  @Output() CancelRegister=new EventEmitter();
  model: any={};
  constructor(private accountservice:AccountService)
  {

  }

  ngOnInit(): void {
    
  }

  register()
  {
    this.accountservice.registerUser(this.model).subscribe(
      {
        next:response=>{
          console.log(response);
          this.cancel();
        },
        error:error=>console.log(error)
      }
    )
  }
  cancel()
  {
    this.CancelRegister.emit(false);
  }
}
