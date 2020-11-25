import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { of, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);

  currentUser$ = this.currentUserSource.asObservable();

  constructor(
    private http: HttpClient,
    private presence: PresenceService,
    private cookieService: CookieService
  ) {}

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model);
  }

  setCurrentUser(user: User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? (user.roles = roles) : user.roles.push(roles);

    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presence.stopHubConnection();
    this.clearCookies();
  }

  getDecodedToken(token) {
    return JSON.parse(atob(token.split('.')[1]));
  }

  sendForgotPasswordEmail(model:any) {
    return this.http.post(this.baseUrl + 'account/forgotpassword' , model);
  }

  resetPassword(model:any) {
    return this.http.post(this.baseUrl + 'account/resetpassword' , model);
  }

  clearCookies() {
    localStorage.removeItem('twoFactorToken');
    localStorage.removeItem('codeExpiry');
    localStorage.removeItem('isSessionActive');
    localStorage.removeItem('attemptsRemaining');
    localStorage.removeItem('codeSendSuccess');
    localStorage.removeItem('user_id');
  }


  confirmEmail(model: any) {
    return this.http.post(this.baseUrl + 'account/confirmemail', model);
  }
}
