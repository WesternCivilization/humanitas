import { Component, Input, OnInit } from '@angular/core';
import { AppSettings } from '../core/app.settings';

@Component({
    templateUrl: './tag.template.html',
    selector: "tag-template"
})
export class TagTemplate implements OnInit {

    @Input()
    public data: any;

    constructor(public _settings: AppSettings) {
    }

    ngOnInit(): void {
    }

    trunc(text: string, length: number): string {
        if (text.length > length)
            return text.substring(0, length) + "..";
        else
            return text;
    }

    public toQueryParams(params:string):any{
        var queryParams = {};
        let args = params.split('&');
        args.forEach(element => {
            let parts = element.split('=');
            queryParams[parts[0]] = parts[1];
        });
        return queryParams;
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