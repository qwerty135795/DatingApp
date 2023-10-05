import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { Observable, async, of } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  constructor(private readonly http:HttpClient,public readonly accountService:AccountService,
    private router:Router,private toastr:ToastrService) {}
  login() {
    this.accountService.login(this.model).subscribe({
      next: () => this.router.navigateByUrl('/members'),
      error: error => this.toastr.error(error.error)
    })
  }
  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
  ngOnInit(): void {
  }
}
