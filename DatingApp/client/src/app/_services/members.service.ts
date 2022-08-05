import { HttpClient } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { Member } from '../_models/member';

// We don't need this since we have jwt interseptor
// const httpOptions = {
//   headers: new HttpHeaders(
//     {
//       Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user'))?.token
//     })
// }

@Injectable({
  providedIn: 'root'
})

export class MembersService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { 

  }

  getMembers(){
    // return this.http.get<Member[]>(this.baseUrl + 'users', httpOptions);   //Old
    return this.http.get<Member[]>(this.baseUrl + 'users');
  }

  getMember(username : string){
    // return this.http.get<Member>(this.baseUrl + 'users/' + username, httpOptions);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

}
