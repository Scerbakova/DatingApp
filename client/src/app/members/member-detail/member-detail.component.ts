import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.scss'],
  imports: [CommonModule, TabsModule, GalleryModule],
})
export class MemberDetailComponent implements OnInit {
  member?: Member;
  images: GalleryItem[] = [];

  constructor(
    private memberService: MembersService,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit() {
    this.loadMember();
  }

  loadMember() {
    const userName = this.activatedRoute.snapshot.paramMap.get('userName');
    if (!userName) return;
    this.memberService.getMember(userName).subscribe((member) => {
      this.member = member;
      this.getImages();
    });
  }

  getImages() {
    if (!this.member) return;
    for (const photo of this.member?.photos) {
      this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }));
    }
  }
}
