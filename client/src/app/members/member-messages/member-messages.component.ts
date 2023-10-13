import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TimeagoModule,FormsModule]
})
export class MemberMessagesComponent implements OnInit {
  @Input()messages:Message[] = [];
  @Input() username?:string;
  @ViewChild('form') messageForm: NgForm | undefined;
  constructor(private messageService:MessageService) {}
  ngOnInit(): void {
  }
  messageContent = '';
  sendMessage() {
    if(!this.username) return;
    this.messageService.sendMessage(this.username,this.messageContent).subscribe({
      next: response => {
        this.messageForm?.reset();
        this.messages.push(response);
      }
    });
  }

}
