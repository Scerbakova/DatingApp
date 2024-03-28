import { Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss'],
})
export class NavComponent {
  @ViewChild('loginForm') loginForm?: NgForm;
  model?: any = {};

  constructor(public accountService: AccountService, private router: Router) {}

  login() {
    this.accountService.login(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
        this.loginForm?.reset();
      },
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
