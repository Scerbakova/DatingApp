import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.scss'],
})
export class TestErrorComponent {
  baseUrl = 'https://localhost:5001/api/';
  validationErrors = [];

  constructor(private https: HttpClient) {}

  get404Error() {
    this.https.get(this.baseUrl + 'buggy/not-found').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.log(error),
    });
  }

  get400Error() {
    this.https.get(this.baseUrl + 'buggy/bad-request').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.log(error),
    });
  }

  get500Error() {
    this.https.get(this.baseUrl + 'buggy/server-error').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.log(error),
    });
  }

  get401Error() {
    this.https.get(this.baseUrl + 'buggy/auth').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.log(error),
    });
  }

  get400ValidationError() {
    this.https
      .post(this.baseUrl + 'account/register', { userName: '', password: '' })
      .subscribe({
        next: (response) => console.log(response),
        error: (error) => {
          console.log(error);
          this.validationErrors = error;
        },
      });
  }
}
