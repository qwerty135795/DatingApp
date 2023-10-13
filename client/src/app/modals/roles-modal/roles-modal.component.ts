import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent {
  username = '';
  selectedRoles:any[] = [];
  availableRoles:any[] = [];
  constructor(public bsModalRef:BsModalRef) {}

  updateChecked(checkValue:string){
    let index = this.selectedRoles.indexOf(checkValue);
    index !== -1 ? this.selectedRoles.splice(index,1) : this.selectedRoles.push(checkValue);
  }
}
