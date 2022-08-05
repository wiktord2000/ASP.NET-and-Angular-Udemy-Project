import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../_models/user';


// Possiblility to inject
@Injectable({
  providedIn: 'root'
})

// Services support Singleton approach so they are present until the app close down
export class AccountService {

  baseUrl = environment.apiUrl;
  // Subject with buffer size (number of elements)
  private currentUserSource = new ReplaySubject<User>(1);
  // Possibility to subscribe
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  login(model: any){
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map( (response: User) => {
        const user = response;
        if(user){
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );
  }

  register(model : any){
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((user: User) => {

        if(user){
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
        // If we want to propagate response to subscriber
        // return user;
      })
    )
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  setCurrentUser(user: User){
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }
}
