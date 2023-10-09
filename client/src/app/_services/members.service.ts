import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members:Member[] = [];
  constructor(private http:HttpClient) { }
  getMembers() {
    if(this.members.length > 0) {
      return of(this.members)
    }
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(map((members:Member[])=> {
       return this.members = members
    }));
  }
  getMember(username:string) {
    const member = this.members.find(u => u.userName === username);
    if(member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }
  updateMember(member:Member) {
    return this.http.put(this.baseUrl + 'users',member).pipe(map(() => {
      const index = this.members.indexOf(member);
      this.members[index] = {...this.members[index],...member}
    }));
  }
  setMainPhoto(id:number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + id,{});
  }
  deletePhoto(photoId:number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId)
  }
}
