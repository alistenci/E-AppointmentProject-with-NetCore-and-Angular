import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { TokenModel } from '../models/token.model';
import { jwtDecode, JwtPayload } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  tokenDecode : TokenModel = new TokenModel();

  constructor(
    private router: Router,
  ) { }

  isAuth(){
    const token : string | null = localStorage.getItem('token');
    if(token){
      console.log("Token:", token); // Token'ı kontrol edin
      const decode: JwtPayload | any = jwtDecode(token);
      console.log("Decoded Token:", decode); // Dekode edilen token'ı kontrol edin

      const expires = decode.expires;
      const now = new Date().getTime() / 1000;
      if(now > expires){
        localStorage.removeItem("token");
        this.router.navigateByUrl("/login");
        return false
      }
      this.tokenDecode.id = decode["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
      this.tokenDecode.name = decode["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
      this.tokenDecode.email = decode["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];
      this.tokenDecode.username = decode["UserName"];
      this.tokenDecode.roles = JSON.parse(decode["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]);
    
      return true;
    }else{
      this.router.navigateByUrl("/login");
      return false; // true ya da false dönmesi gerekir. Yoksa canActiveChild çalışmaz.
    }
  }
}
