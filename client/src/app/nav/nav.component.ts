import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss'],
})
export class NavComponent {
  @ViewChild('loginForm') loginForm?: NgForm;
  model?: any = {};

  constructor(
    public accountService: AccountService,
    private router: Router,
    private memberService: MembersService
  ) {}

  login() {
    this.memberService.resetUserParams();
    this.accountService.login(this.model).subscribe(() => {
      this.router.navigateByUrl('/members');
      this.loginForm?.reset();
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
