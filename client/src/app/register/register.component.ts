import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  @Input() usersFormComponent:any;
  @Output() CancelRegister=new EventEmitter();
  model: any={};
  constructor(private accountservice:AccountService,private toastr:ToastrService)
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
        error:error=>this.toastr.error(error.error)
      }
    )
  }
  cancel()
  {
    this.CancelRegister.emit(false);
  }
}
