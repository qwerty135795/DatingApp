import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { TimeagoModule } from 'ngx-timeago'
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs',{static:true}) memberTabs?: TabsetComponent;
  activateTab?: TabDirective;
  member: Member = {} as Member;
  messages:Message[] = [];
  images: GalleryItem[] = [];
  constructor(private memberService: MembersService, private route: ActivatedRoute,
    private messageService: MessageService) { }
  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => this.member = data['member']
    });
    this.loadImages();
    this.route.queryParams.subscribe({
      next: response => response['tab'] && this.selectTabs(response['tab'])
    })
  }


  onTabActivated(data:TabDirective) {
    this.activateTab = data;
    if(data.heading === 'Messages'){
      this.loadMessages();
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
