import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users:User[] = [];
  availableRoles = [
    'Admin', 'Moderator', 'Member'
  ];
  modalRef:BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();
  constructor(private adminService:AdminService,private modalService:BsModalService) {}
  ngOnInit(): void {
      this.loadUsers();
  }
  loadUsers() {
    this.adminService.getUsersWithRoles().subscribe({
      next: users => this.users = users
    })
  }

  showModal(user:User) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        username: user.username,
        selectedRoles: [...user.roles],
        availableRoles: this.availableRoles
      }
    };
    this.modalRef = this.modalService.show(RolesModalComponent,config);
    this.modalRef.onHidden?.subscribe({
      next: () => {
        const selectRoles = this.modalRef.content!.selectedRoles;
        if(!this.roleEqual(selectRoles, user.roles)) {
          this.adminService.updateUserRoles(user.username, selectRoles).subscribe({
            next: roles => user.roles = roles
          });
        }
      }
    })
  }


  private roleEqual(arr1:any[],arr2:any[]){
    return JSON.stringify(arr1.sort()) === JSON.stringify(arr2.sort());
  }
}
