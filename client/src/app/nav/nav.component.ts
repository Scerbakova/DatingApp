import { Component } from '@angular/core';
import { LoginRegisterFormModel } from '../_models/loginRegisterModel';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss'],
})
export class NavComponent {
  model: LoginRegisterFormModel = {
    userName: '',
    password: '',
  };

  constructor(
    public accountService: AccountService,
    private router: Router,
  ) {}

  login() {
    this.accountService.login(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
        this.model.userName = '';
        this.model.password = '';
      },
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
