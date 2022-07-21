import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

// Decorator (starting with @)
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
  title = 'The dating app';
  users: any;

  // Constructor injection in Angular
  constructor(private http: HttpClient){

  }

  // Starts after constructor
  ngOnInit(){
    this.getUsers();
  }

  getUsers() {
    this.http.get('https://localhost:5001/api/users').subscribe(response => {
      this.users = response;
    }, error => {
      console.log(error);
    })
  }

}