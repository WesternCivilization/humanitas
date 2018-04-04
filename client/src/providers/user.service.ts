import { Injectable } from '@angular/core';
import { Http, RequestOptions, Headers } from '@angular/http';
import { AppSettings } from '../core/app.settings';
import { Observable } from 'rxjs/Observable';
import { IOption } from 'ng-select';
import { TimelineActivity } from './timelineactivity';

@Injectable()
export class UserService {

    constructor(private _http: Http, public _settings: AppSettings) {
    }

    detail(id: string): any {
        let url = this._settings.ApiUrl + '/api/user/detail?userId=' + id + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

    metrics(id: string): any {
        let url = this._settings.ApiUrl + '/api/user/metrics?userId=' + id + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

   users(start: Number): Observable<any> {
        let url = this._settings.ApiUrl + '/api/user/list?start=' + start + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

}