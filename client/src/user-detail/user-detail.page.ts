import { Component, ViewChild, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { Http, Response } from '@angular/http';
import { ActivatedRoute } from '@angular/router';
import { TimelineActivity } from '../providers/timelineactivity';
import { AppSettings } from '../core/app.settings';
import { Breadcrumb } from '../core/breadcrumb';
import { TopicaService } from '../providers/topica.service';
import { ZeteticaService } from '../providers/zetetica.service';
import { Subscription } from 'rxjs/Subscription';
import { UserService } from '../providers/user.service';

@Component({
    templateUrl: './user-detail.page.html'
})
export class UserDetailPage implements OnInit {

    data: any;
    id: string;
    sub: Subscription;
    sub2: Subscription;
    metrics: any;

    constructor(public _settings: AppSettings,
        private _route: ActivatedRoute,
        public _breadcrumb: Breadcrumb,
        private _userService: UserService) {
    }

    ngOnInit(): void {

        this._route.params.subscribe(params => {
            if (params["id"]) {
                this._breadcrumb.clear();
                this._breadcrumb.append("Usuários", "fa fa-users", "/users");

                this.id = params["id"];
                if(this.sub)
                    this.sub.unsubscribe();
                this.sub = this._userService.detail(this.id)
                    .subscribe(detail => {
                        this.data = detail;
                        this._breadcrumb.append(this.data.name, "fa fa-user", "/users/" + this.data.userId);
                    });
                    if(this.sub2)
                    this.sub2.unsubscribe();
                this.sub2 = this._userService.metrics(this.id)
                    .subscribe(metrics => {
                        this.metrics = metrics;
                    });
            }
        });
    }

    trunc(text: string, length: number): string {
        if (text.length > length)
            return text.substring(0, length) + "..";
        else
            return text;
    }

    public formatText(text: string): string {
        return this._settings.format(text);
    }

    public selectText(ev: any): void {
        var txt = ev.target;
        var selection = window.getSelection();
        var range = document.createRange();
        range.selectNodeContents(txt);
        selection.removeAllRanges();
        selection.addRange(range);
    }


}