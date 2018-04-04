import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import 'rxjs/add/operator/map';
import { Breadcrumb } from '../core/breadcrumb';
import { IOption } from 'ng-select';

import {
  AuthService,
  FacebookLoginProvider,
  GoogleLoginProvider,
  LinkedinLoginProvider,
  SocialUser
} from 'ng4-social-login';
import { AppSettings } from '../core/app.settings';
import { LoginService } from './login.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {

  public searching: boolean;
  public user: any;
  public loggedIn: boolean;

  constructor(public _breadcrumb: Breadcrumb,
    public authService: AuthService,
    public route: Router,
    public _settings: AppSettings,
    public _login: LoginService) {
  }

  ngOnInit() {
    if (this._settings.ApiUrl != "http://rafaelmelo.web1612.kinghost.net/humanitasapi") {
      this.user = {
        email: "rafaelmelo007@gmail.com",
        id: "10155330534424017",
        name: "Rafael Melo",
        photoUrl: "https://graph.facebook.com/10155330534424017/picture?type=normal",
        provider: "FACEBOOK",
        token: "EAAE6keP0ZCAABAIEVkAOc0C6NhkZAUA4WDVkfoNkzZCSoBV2ZCjXZBMIpjwz0ytCveodG0Eh98m5l4OHPVDLZAWZAl6YidNsW2XkIGdZACz0ioLvX9IpcSidJbp4FRlZCU0B46KsZCtbUtspmFd5VGZBHyec3cjtKozD4mLKqvdYTGkK8mGYllZCSZAaQhzkwnP4EDkjxWTGr2HzWMAZDZD"
      };
      this._login.login(this.user).subscribe(f => {
        AppSettings.UserToken = this.user.token;
        this.user.userId = f.split(';')[0];
        this.user.userTypeId = f.split(';')[1];
        AppSettings.UserTypeId = f.split(';')[1];
        console.warn("Login as:");
        console.warn(this.user);
        console.warn(f);
      });
    }
    else {
      this.authService.authState.subscribe((user) => {
        this.user = user;
        this.loggedIn = (user != null);
        if (!this.loggedIn) {
          this.route.navigate(['/login']);
        }
        else {
          this._login.login(this.user).subscribe(f => {
            AppSettings.UserToken = this.user.token;
            this.user.userId = f.split(';')[0];
            this.user.userTypeId = f.split(';')[1];
            AppSettings.UserTypeId = f.split(';')[1];
            console.warn("Login as:");
            console.warn(this.user);
            console.warn(f);
          });
        }
      });
    }
  }

  filter(options: Array<IOption>): void {
    if (options) {
      this._breadcrumb.applyFilters(options);
    }
  }

  signOut(): void {
    if (this.user != null) {
      this._login.logout(this.user).subscribe(f => f);
      this.authService.signOut();
      this.user = null;
    }
    this.route.navigate(['/login']);
  }
}
