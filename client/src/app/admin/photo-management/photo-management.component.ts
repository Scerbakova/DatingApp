import { Component, OnInit } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.scss'],
})
export class PhotoManagementComponent implements OnInit {
  photos: Photo[] = [];
  constructor(private adminService: AdminService) {}

  ngOnInit() {
    this.getPhotosForApproval();
  }

  getPhotosForApproval() {
    this.adminService
      .getPhotosForApproval()
      .subscribe((photos) => (this.photos = photos));
  }

  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe(() => {
      this.removeFromListOfPhotosToApprove(photoId);
    });
  }

  rejectPhoto(photoId: number) {
    this.adminService.rejectPhoto(photoId).subscribe();
    this.removeFromListOfPhotosToApprove(photoId);
  }

  removeFromListOfPhotosToApprove(photoId: number) {
    const index = this.photos.findIndex((photo) => photo.id === photoId);
    this.photos.splice(index, 1);
  }
}
