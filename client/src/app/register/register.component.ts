import { Component, EventEmitter, Output } from '@angular/core';
import { LoginRegisterFormModel } from '../_models/loginRegisterModel';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
  @Output() cancelRegister = new EventEmitter();
  model: LoginRegisterFormModel = {
    username: '',
    password: '',
  };

  constructor(private accoutService: AccountService) {}

  register() {
    this.accoutService.register(this.model).subscribe({
      next: () => this.cancelRegisterMode(),
      error: (error) => console.log(error),
    });
  }

  cancelRegisterMode() {
    this.cancelRegister.emit(false);
  }
}
