import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {}
  loggedIn: boolean = true;
  // currentUser$: Observable<User>;

  constructor(public accountService: AccountService){

  }

  ngOnInit(): void {
    // // Track the user change
    // this.getCurrentUser();
    // this.currentUser$ = this.accountService.currentUser$;
  }

  login(){

    this.accountService.login(this.model).subscribe({
      next: (response) => {
        console.log(response);
      },
      error: (error) => {
        console.log(error);
      }
    }); 
  }

  logout(): void{
    this.accountService.logout();
  }

  // getCurrentUser(){
  //   this.accountService.currentUser$.subscribe({
  //     next: (user) => {
  //       this.loggedIn = !!user;
  //     }
  //   })
  // }

}
