import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubsUrl;
  private hubConnection?: HubConnection;
  private onlineUsersSource: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();
  constructor(private toastr: ToastrService,private router:Router) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      }).withAutomaticReconnect().build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: (usersOnline) => this.onlineUsersSource.next([...usersOnline,username])
      })
    });

    this.hubConnection.on('UserIsDisconnected', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: onlineUsers =>
        this.onlineUsersSource
        .next(onlineUsers.filter((el) => el !== username))
      })
    })

    this.hubConnection?.on('GetOnlineUsers', usernames => {
      this.onlineUsersSource.next(usernames);
    })
    this.hubConnection.on('NewMessageReceived',({username,knownAs}) => {
      this.toastr.info(knownAs + ' send your a new message.click me to see it').onTap
      .pipe(take(1)).subscribe({
        next: () => this.router.navigateByUrl('/members/' + username + '?tab=Messages')
      });
    })

  }
  stopHunConnection() {
    this.hubConnection?.stop().catch(error => console.log(error))
  }
}
