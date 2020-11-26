import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'The Dating App';
  users: any;

  constructor(
    private accountService: AccountService,
    private presence: PresenceService
  ) {}
  ngOnInit() {
    this.setCurrentUser();
  }

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (user) {
      this.accountService.setCurrentUser(user);
      this.presence.createHubConnection(user);
      this.onBeforeUnload();
      this.OnUnload();
    }
  }
  onBeforeUnload() {
    let context = this;
    window.addEventListener('beforeunload', function (e) {
      let user: User = JSON.parse(localStorage.getItem('user'));
      if (user) {
        context.logoutOnClose();
      }
    });
  }

  OnUnload() {
    let context = this;
    window.addEventListener('onunload', function (e) {
      let user: User = JSON.parse(localStorage.getItem('user'));
      if (user) {
        context.logoutOnClose();
      }
    });
  }

  logoutOnClose() {
    this.accountService.logout();
  }
}
