import { Component, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { Http, Response } from '@angular/http';
import { Breadcrumb } from '../core/breadcrumb';
import { IOption } from 'ng-select';
import { TimelineActivity } from '../providers/timelineactivity';
import { AppSettings } from '../core/app.settings';
import { DomSanitizer } from '@angular/platform-browser';
import { Subscription } from 'rxjs/Subscription';
import { UserService } from '../providers/user.service';

@Component({
    templateUrl: './users.page.html'
})
export class UsersPage implements OnInit, OnDestroy {

    users: Array<any>;
    usersCount: number;
    sub: Subscription;
    isLoading: boolean;

    constructor(public _breadcrumb: Breadcrumb,
        public _settings: AppSettings,
        private _route: ActivatedRoute,
        private _userService: UserService,
        private _domSanitizer: DomSanitizer) {
    }

    ngOnInit(): void {
        this._route.params.subscribe(params => {
            this.filter(new Array());
        });
        this._breadcrumb.clear();
        this._breadcrumb.append("Usuários", "fa fa-users", "/users");
    }

    ngOnDestroy(): void {
        if (this.sub) this.sub.unsubscribe();
    }

    trunc(text: string, length: number): string {
        if (text.length > length)
            return text.substring(0, length) + "..";
        else
            return text;
    }

    filter(ev: Array<IOption>): void {
        this.users = new Array();
        if(this.sub)
        {
            this.sub.unsubscribe();
        }
        this.sub = this._userService.users(0).subscribe(users => {
            this.users = this.users.concat(users.rows);
            this.usersCount = users.totalOfRecords;
        });
    }

    url(src:string){
        return this._domSanitizer.bypassSecurityTrustStyle(src);
    }

    onScroll() {
        if (this.isLoading) return;
        this.isLoading = true;
        if(this.sub)
        {
            this.sub.unsubscribe();
        }
        this.sub = this._userService.users(this.users.length).subscribe(users => {
            this.users = this.users.concat(users.rows);
            this.usersCount = users.totalOfRecords;
            this.isLoading = false;
        });
    }

}