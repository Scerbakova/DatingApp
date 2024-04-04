import { Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { NgForm } from '@angular/forms';
import { MembersService } from '../_services/members.service';
import { User } from '../_models/user';

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
    public memberService: MembersService
  ) {}

  login() {
    this.accountService.login(this.model).subscribe(() => {
      this.memberService.getUser();
      this.router.navigateByUrl('/members');
      this.loginForm?.reset();
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
