import { Component } from '@angular/core';
import { LoginRegisterFormModel } from '../_models/loginRegisterModel';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss'],
})
export class NavComponent {
  model: LoginRegisterFormModel = {
    username: '',
    password: '',
  };

  constructor(public accountService: AccountService) {}

  login() {
    this.accountService.login(this.model).subscribe({
      next: (response) => {
        console.log(response);
        this.model.username = '';
        this.model.password = '';
      },
      error: (error) => console.log(error),
    });
  }

  logout() {
    this.accountService.logout();
  }
}
