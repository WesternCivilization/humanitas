import { Injectable, EventEmitter } from '@angular/core';
import { Http, RequestOptions, Headers } from '@angular/http';
import { AppSettings } from '../core/app.settings';
import { Observable } from 'rxjs/Observable';
import { IOption } from 'ng-select';
import { TimelineActivity } from './timelineactivity';

@Injectable()
export class ZeteticaService {

    public static saved: EventEmitter<any> = new EventEmitter();

    constructor(private _http: Http, public _settings: AppSettings) {

    }

    save(data: any): Observable<any> {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this._http
            .post(this._settings.ApiUrl + '/api/zetetica/save?token=' + AppSettings.UserToken, data, options)
            .map((response: any) => response.json());
    }

    notifyAll(data: any): void {
        ZeteticaService.saved.emit(data);
    }

    batchdata(type: string, tag: string): Observable<any> {
        let url = this._settings.ApiUrl + '/api/zetetica/batchdata?type=' + type + '&tag=' + tag + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

    activities(tag: string, sort: string, start: Number): Observable<any> {
        let url = this._settings.ApiUrl + '/api/zetetica/activities?tag=' + tag + '&sort=' + sort + '&start=' + start + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

    fragment(fragmentId: String): any {
        let url = this._settings.ApiUrl + '/api/zetetica/fragment?fragmentId=' + fragmentId + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

    fragments(domainId: number, tagId: String, start: Number): Observable<any> {
        let url = this._settings.ApiUrl + '/api/zetetica/fragments?domainId=' + domainId + '&tagId=' + tagId + '&start=' + start + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

    detail(id: string): any {
        let url = this._settings.ApiUrl + '/api/zetetica/detail?fragmentId=' + id + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

    score(fragmentId: string, score: number): Observable<boolean> {
        let url = this._settings.ApiUrl + '/api/zetetica/score?fragmentId=' + fragmentId + '&score=' + score + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

    listen(fragmentId: string): Observable<boolean> {
        let url = this._settings.ApiUrl + '/api/zetetica/listen?fragmentId=' + fragmentId + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

}