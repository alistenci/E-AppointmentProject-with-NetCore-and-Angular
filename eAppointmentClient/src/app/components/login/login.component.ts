import { Component, ElementRef, ViewChild, viewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { LoginModel } from '../../models/login.model';
import { HttpService } from '../../services/http.service';
import { LoginResponseModel } from '../../models/login-response.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule,],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

  login: LoginModel = new LoginModel();

  @ViewChild("passwordInput") passwordInput : ElementRef<HTMLInputElement> | undefined;

  constructor(
    private _http: HttpService,
    private router: Router
  ){}

  // showOrHidePassword(){
  //   if(this.passwordInput === undefined) return;
    
  //   if(this.passwordInput.nativeElement.type === "password"){
  //     this.passwordInput.nativeElement.type = "text";
  //   }else{
  //     this.passwordInput.nativeElement.type = "password";
  //   }
  // }

  signIn(form:NgForm){
    if(form.valid){
      this._http.post<LoginResponseModel>("Auth/Login", this.login,(res)=> {
        localStorage.setItem("token", res.data!.token);
        this.router.navigateByUrl("/");
      })
    }
  }

}
