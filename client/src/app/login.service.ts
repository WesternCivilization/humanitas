import { Injectable, OnInit, NgZone } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { AppSettings } from '../core/app.settings';
import { Observable } from 'rxjs/Observable';
import { IOption } from 'ng-select';

@Injectable()
export class LoginService {

    constructor(private _http: Http,
        public _settings: AppSettings) {
    }

    login(data: any): Observable<any> {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this._http
            .post(this._settings.ApiUrl + '/api/user/login', data, options)
            .map((response: any) => response.json());
    }

    logout(data: any): Observable<any> {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this._http
            .post(this._settings.ApiUrl + '/api/user/logout', data, options)
            .map((response: any) => response.json());
    }

}

