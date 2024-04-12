import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { initialState } from 'ngx-bootstrap/timepicker/reducer/timepicker.reducer';
import { Role } from 'src/app/_models/roles';
import { UserWithRole } from 'src/app/_models/userWithRole';
import { AdminService } from 'src/app/_services/admin.service';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss'],
})
export class UserManagementComponent implements OnInit {
  users: UserWithRole[] = [];
  bsModalRef?: BsModalRef<RolesModalComponent> =
    new BsModalRef<RolesModalComponent>();
  availableRoles: Role[] = ['Admin', 'Moderator', 'Member'];

  constructor(
    private adminService: AdminService,
    private modalService: BsModalService
  ) {}
  ngOnInit() {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService
      .getUsersWithRoles()
      .subscribe((users) => (this.users = users));
    this.users.map((user) => console.log('user', user));
  }

  openRolesModal(user: UserWithRole) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        username: user.username,
        availableRoles: this.availableRoles,
        selectedRoles: [...user.roles],
      },
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    this.bsModalRef.onHide?.subscribe(() => {
      const selectedRoles = this.bsModalRef?.content?.selectedRoles;
      if (
        selectedRoles?.length &&
        !this.arrayEqual(selectedRoles, user.roles)
      ) {
        this.adminService
          .updateUserRoles(user.username, selectedRoles)
          .subscribe((roles) => {
            user.roles = roles;
          });
      }
    });
  }

  private arrayEqual(arr1: string[], arr2: string[]) {
    return JSON.stringify(arr1?.sort()) === JSON.stringify(arr2.sort());
  }
}
