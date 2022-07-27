import { ToastrService } from 'ngx-toastr';

import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error) => {
        if(error){
          switch(error.status){

            // Case 400
            case 400:
              if(error.error.errors){

                const modelStateErrors = [];
                for(const key in error.error.errors){
                  modelStateErrors.push(error.error.errors[key]);
                }
                throw modelStateErrors.flat();

              }else{
                this.toastr.error(error.statusText, error.status);
              }
              break;

            // Case 401
            case 401:
              this.toastr.error(error.statusText, error.status);
              break;

            //Case 404
            case 404:
              this.router.navigateByUrl('/not-found');
              break;

            //Cse 500
            case 500:
              const navigationsExtras: NavigationExtras = {state: {error: error.error}}
              this.router.navigateByUrl('/server-error', navigationsExtras);
              break;
            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
              break;
          }

          return throwError(error);
        }
      })
    );
  }
}
