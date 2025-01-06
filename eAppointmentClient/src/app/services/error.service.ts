import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SwalService } from './swal.service';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {

  constructor(
    private swal: SwalService
  ) { }

  errorHandler(err: HttpErrorResponse){
    console.error(err);
    let message = "Error!";

    if(err.status === 0){
      message = "API is not available"
    }else if(err.status === 404){
      message = "Not Found"
    } else if(err.status === 401){
      message = "Unauthorized"
    }else if(err.status === 500){
      message = "";
        for(const e of err.error.errorMessages){
        message += e + "\n";
      }
    }


    this.swal.callToast(message, "error");
  }
}
