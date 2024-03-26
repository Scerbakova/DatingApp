import { Component, EventEmitter, Output } from '@angular/core';
import { LoginRegisterFormModel } from '../_models/loginRegisterModel';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
  @Output() cancelRegister = new EventEmitter();
  model: LoginRegisterFormModel = {
    userName: '',
    password: '',
  };

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
    this.model.userName = '';
    this.model.password = '';
    this.cancelRegister.emit(false);
  }
}
