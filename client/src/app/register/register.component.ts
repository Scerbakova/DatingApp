import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
  @ViewChild('registerForm') registerForm?: NgForm;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};

  constructor(
    private accoutService: AccountService,
    private toastr: ToastrService
  ) {}

  register() {
    this.accoutService.register(this.model).subscribe({
      next: () => this.cancelRegisterMode(),
      error: ({ error }) => this.toastr.error(error.title),
    });
  }

  cancelRegisterMode() {
    this.registerForm?.reset();
    this.cancelRegister.emit(false);
  }
}
