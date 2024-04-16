import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { UserParams } from 'src/app/_models/userParams';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.scss'],
})
export class MemberListComponent implements OnInit {
  members: Member[] = [];
  pagination?: Pagination;
  userParams?: UserParams | null;
  genderList = [
    { value: 'male', displayValue: 'Males' },
    { value: 'female', displayValue: 'Females' },
  ];

  constructor(private memberService: MembersService) {}

  ngOnInit() {
    this.getParams();
    this.loadMembers();
  }

  resetFilters() {
    this.memberService.resetUserParams();
    this.getParams();
    this.loadMembers();
  }

  getParams() {
    this.memberService.userParams$.pipe(take(1)).subscribe((userParams) => {
      this.userParams = userParams;
    });
  }

  loadMembers() {
    if (this.userParams) {
      this.memberService.getMembers(this.userParams).subscribe({
        next: ({ result, pagination }) => {
          if (result && pagination) {
            this.members = result;
            this.pagination = pagination;
          }
        },
      });
    }
  }

  pageChanged(e: any) {
    if (this.userParams && this.userParams?.pageNumber !== e.page) {
      this.userParams.pageNumber = e.page;
      this.loadMembers();
    }
  }
}
