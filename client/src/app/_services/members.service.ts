import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { Pagination, PaginationResult } from '../_models/pagination';
import { UserParam } from '../_models/userParam';
import { AccountService } from './account.service';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  User:User | undefined;
  UserParams:UserParam | undefined;
  constructor(private http: HttpClient,private accountService:AccountService) {
    accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if(!user) return;
        this.UserParams = new UserParam(user);
        this.User = user;
      }
    })
  }
  getUserParams() {
    return this.UserParams;
  }
  setUserParams(userParams:UserParam) {
    this.UserParams = userParams;
  }
  resetUserParams() {
    if(this.User) {
      this.UserParams = new UserParam(this.User);
      return this.UserParams;
    }
    return;
  }
  getMembers(userParam: UserParam) {
    const values = this.memberCache.get(Object.values(userParam).join('-'));
    if(values) return of(values);

    let params = this.getPaginationHeaders(userParam.pageNumber, userParam.pageSize);
    params = params.append('gender',userParam.gender);
    params = params.append('minAge',userParam.minAge);
    params = params.append('maxAge',userParam.maxAge);
    params = params.append('orderBy',userParam.orderBy)
    return this.getPaginationResult<Member[]>(this.baseUrl + 'users',params).pipe(map(response => {
      this.memberCache.set(Object.values(userParam).join('-'),response);
      return response;
    }))
  }


  getMember(username: string) {
    const member = [...this.memberCache.values()]
    .reduce((arr,elem) => arr.concat(elem.result),[])
    .find((member:Member) => member.userName === username);
    if(member) return of(member);
    if( this.members.find(user => user.userName == username)) return of(this.members.find(u => u.userName === username))
    return this.http.get<Member>(this.baseUrl + 'users/' + username).pipe(map(user => {
      this.members.push(user);
      return of(user)
    }));
  }
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(map(() => {
      const index = this.members.indexOf(member);
      this.members[index] = { ...this.members[index], ...member }
    }));
  }
  setMainPhoto(id: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + id, {});
  }
  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId)
  }
  addLike(userName:string) {
    return this.http.post(this.baseUrl + 'likes/'+ userName,{});
  }
  getLikes(predicate:string,pageNumber:number,pageSize:number) {
    let params = this.getPaginationHeaders(pageNumber,pageSize);
    params = params.append('predicate',predicate);
    return this.getPaginationResult<Member[]>(this.baseUrl + 'likes',params);
  }


  private getPaginationResult<T>(url:string,params: HttpParams) {
    const paginationResult:PaginationResult<T> = new PaginationResult<T>;
    return this.http.get<T>(url, { observe: 'response', params }).pipe(map(response => {
      if (response.body) {
        paginationResult.result = response.body;
      }
      const paginationHead = response.headers.get('Pagination');
      if (paginationHead) {
        paginationResult.pagination = JSON.parse(paginationHead);
      }
      return paginationResult;
    }));
  }

  private getPaginationHeaders(pageNumber:number,pageSize:number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber);
    params = params.append("pageSize", pageSize);
    return params;
  }
}
