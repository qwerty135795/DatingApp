import { Component, OnInit } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import { map, pipe } from 'rxjs';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.css']
})
export class TestErrorComponent implements OnInit {
  baseUrl = 'https://localhost:5001/api/';
  constructor(private http:HttpClient) {}
  validationErrors:string[] = [];
  ngOnInit(): void {
  }
  get400(){
    this.http.get(this.baseUrl + 'buggy/badReq').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    });
  }
  get401() {
    this.http.get(this.baseUrl + 'buggy/auth').subscribe({
      next:response => console.log(response),
      error: error => console.log(error)
    })
  }
  get404() {
    this.http.get(this.baseUrl + 'buggy/not-found').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    })
  }
  get500() {
    this.http.get(this.baseUrl + 'buggy/server-error').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    })
  }
  get400Validation() {
    this.http.post(this.baseUrl + 'account/register',{}).subscribe({
      next: response => console.log(response),
      error: error => {
        console.log(error);
        this.validationErrors = error;
      }
    })
  }
}
