import { Component, OnInit } from '@angular/core';
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
  userParams?: UserParams;
  genderList = [
    { value: 'male', displayValue: 'Males' },
    { value: 'female', displayValue: 'Females' },
  ];

  constructor(private memberService: MembersService) {}

  ngOnInit() {
    this.userParams = this.memberService.getUserParams();
    this.loadMembers();
  }

  resetFilters() {
    this.userParams = this.memberService.resetUserParams();
    this.loadMembers();
  }

  loadMembers() {
    if (this.userParams) {
      this.memberService.setUserParams(this.userParams);
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
      this.memberService.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
