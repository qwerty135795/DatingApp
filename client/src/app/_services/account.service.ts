import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, map } from "rxjs";
import { User } from "../_models/user";
import { Router } from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class AccountService{
  baseUrl = 'https://localhost:5001/api/';
  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();
  constructor(private readonly http:HttpClient,private readonly router:Router) {}

  login(model:any){
    return this.http.post<User>(this.baseUrl + 'account/login',model).pipe(map((response:User) => {
      const user = response;
      if(user) {
        localStorage.setItem('user',JSON.stringify(user));
        this.currentUserSource.next(user);
      }
    }));
  }
  setCurrentUser(user:User) {
    this.currentUserSource.next(user);
  }
  register(model:any) {
    return this.http.post<User>(this.baseUrl + 'account/register',model).pipe(map((response) => {
      if(response) {
        localStorage.setItem('user',JSON.stringify(response));
        this.currentUserSource.next(response);
      }
    }))
  }

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
