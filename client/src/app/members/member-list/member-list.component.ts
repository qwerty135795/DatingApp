import { Component, OnInit } from '@angular/core';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParam } from 'src/app/_models/userParam';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  //members$:Observable<Member[]> | undefined;
  members: Member[] | undefined;
  pagination: Pagination | undefined;
  UserParams: UserParam | undefined;
  user:User |undefined;
  genderList = [{ value: 'male', display: 'Males' },
  { value: 'female', display: 'Females' }]
  constructor(public membersService: MembersService) {
    this.UserParams = this.membersService.getUserParams();
  }

  ngOnInit(): void {
    this.loadMembers();
  }
  pageChange(event: PageChangedEvent) {
    if (this.UserParams && this.UserParams?.pageNumber !== event.page) {
      this.UserParams.pageNumber = event.page;
      this.membersService.setUserParams(this.UserParams);
      this.loadMembers();
    }
  }
  resetFilters() {
    this.UserParams = this.membersService.resetUserParams();
  }
  loadMembers() {
    if (this.UserParams) {
      console.log(this.UserParams);
      this.membersService.getMembers(this.UserParams).subscribe({
        next: response => {
          if (response) {
            this.pagination = response.pagination;
            this.members = response.result
          }
        }
      })
    }

  }
}
