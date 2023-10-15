import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { TimeagoModule } from 'ngx-timeago'
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';
import { PresenceService } from 'src/app/_services/presence.service';
import { AccountService } from 'src/app/_services/account.service';
import { take } from 'rxjs';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs',{static:true}) memberTabs?: TabsetComponent;
  activateTab?: TabDirective;
  member: Member = {} as Member;
  messages:Message[] = [];
  images: GalleryItem[] = [];
  user:User | null = null;
  constructor(private accountService: AccountService, private route: ActivatedRoute,
    private messageService: MessageService,public presenceService:PresenceService) { }
  ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next:user => this.user = user
    })
    this.route.data.subscribe({
      next: data => this.member = data['member']
    });
    this.loadImages();
    this.route.queryParams.subscribe({
      next: response => response['tab'] && this.selectTabs(response['tab'])
    })
  }

  ngOnDestroy(): void {
      this.messageService.stopHubConnection();
  }
  onTabActivated(data:TabDirective) {
    this.activateTab = data;
    if(data.heading === 'Messages' && this.user){
      this.messageService.createHubConnection(this.user,this.member.userName);
  } else {
    this.messageService.stopHubConnection();
  }
}
  loadMessages(){
    if (!this.member) return;
    this.messageService.getMessageThread(this.member.userName).subscribe({
      next: response => this.messages = response
    })
  }
  selectTabs(heading:string) {
    if(this.memberTabs) {
      this.memberTabs.tabs.find(t => t.heading === heading)!.active = true;
      console.log(this.activateTab!.heading);
    }
  }
  loadImages() {
    if (!this.member) return;
    for (const photo of this.member.photos) {
      this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }))
    }
  }
}
