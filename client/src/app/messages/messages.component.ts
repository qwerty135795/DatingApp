import { Component, OnInit } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
   messages?:Message[];
  pagination?:Pagination;
  pageNumber = 1;
  pageSize = 5;
  container = 'Unread';
  loading = false;
  constructor(private messageService:MessageService) {}
  ngOnInit(): void {
    this.loadMessages();
  }
  loadMessages() {
    this.loading = true;
    this.messageService.getMessages(this.pageNumber,this.pageSize,this.container).subscribe({
      next: response => {
        this.messages = response.result,
        this.pagination = response.pagination,
        this.loading = false
      }
    })
  }
  pageChange(event:any) {
    if(this.pageNumber !== event ) {
      this.pageNumber = event;
    }
  }
  deleteMessage(id:number) {
    if(!this.messages) return;
    this.messageService.deleteMessage(id).subscribe({
      next: () => this.messages?.splice(this.messages.findIndex(m => m.id === id),1)
    })
  }
}
