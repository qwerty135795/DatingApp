import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyCount = 0;
  constructor(private spinnerService:NgxSpinnerService) { }

  busy() {
    this.busyCount++;
    this.spinnerService.show(undefined, {
      bdColor: 'rgba(255,255,255,0)',
      color: 'pink'
    });
  }

  complete() {
    this.busyCount--;
    if(this.busyCount <= 0) {
      this.busyCount = 0;
      this.spinnerService.hide();
    }
  }
}
