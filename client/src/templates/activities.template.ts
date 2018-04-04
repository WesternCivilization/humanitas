import { Component, Input, Output, EventEmitter } from '@angular/core';
import { AppSettings } from '../core/app.settings';
import { Player } from '../core/player';

@Component({
    templateUrl: './activities.template.html',
    selector: "activities-template"
})
export class ActivitiesTemplate {

    @Input()
    public data: Array<any>;

    @Input()
    public tag: string;

    @Input()
    public sortType: string;

    @Output()
    public filter: EventEmitter<string> = new EventEmitter();

    @Output()
    public sort: EventEmitter<string> = new EventEmitter();

    constructor(public _settings: AppSettings,
        public _player: Player) {
            this.tag = AppSettings.FilterTag;
            this.sortType = AppSettings.SortType;
    }

    public setFilter(value: string): void {
        this.filter.emit(value);
        this.tag = value;
        AppSettings.FilterTag = value;
    }

    public setSort(value: string): void {
        this.sort.emit(value);
        this.sortType = value;
        AppSettings.SortType = value;
    }

    public toQueryParams(params: string): any {
        var queryParams = {};
        let args = params.split('&');
        args.forEach(element => {
            let parts = element.split('=');
            queryParams[parts[0]] = parts[1];
        });
        return queryParams;
    }

    public trunc(text: string, length: number): string {
        if (text && text.length > length)
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