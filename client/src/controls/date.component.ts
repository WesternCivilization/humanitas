import { Component, ViewChild, OnInit, OnChanges, Output, Input, EventEmitter } from '@angular/core';

import { TopicaService } from '../providers/topica.service';
import { Subscription } from 'rxjs/Subscription';
import { } from '@angular/core/src/metadata/lifecycle_hooks';

@Component({
    templateUrl: './date.component.html',
    selector: "date-control"
})
export class DateComponent implements OnInit, OnChanges {

    @Input("label")
    public label: string;

    @Input("value")
    public value: string = "";

    @Input("placeholder")
    public placeholder: string;

    @Input("icon")
    public icon: string;

    @Output("change")
    public change: EventEmitter<any> = new EventEmitter();

    type: string;
    date: string;
    year: number;
    century: number;

    constructor(private _topicaService: TopicaService) {
    }

    ngOnInit(): void {
        let parts = this.value.split('|');
        if (parts[0] != "") {
            this.type = "date";
            this.date = parts[0].split('T')[0];
            if (this.date == "0001-01-01") {
                this.date = "";
            }
        }
        else if (parts.length > 1 && parts[1] != "") {
            this.type = "year";
            this.year = +parts[1];
        }
        else if (parts.length > 2 && parts[2] != "") {
            this.type = "century";
            this.century = +parts[2];
        }
        else {
            this.type = "date";
            this.date = "";
        }
    }

    ngOnChanges(): void {
        this.ngOnInit();
    }

    setValue(type: string, ev: any): void {
        if (ev && ev.currentTarget) {
            if (type == "date") {
                this.date = ev.currentTarget.value;
                this.year = null;
                this.century = null;
            }
            else if (type == "year") {
                this.date = null;
                this.year = ev.currentTarget.value;
                this.century = null;
            }
            else if (type == "century") {
                this.date = null;
                this.year = null;
                this.century = ev.currentTarget.value;
            }
            this.change.emit((this.date ? this.date : "") + "|" +
            (this.year ? this.year : "") + "|" +
            (this.century ? this.century : ""));
        }
        ev.stopPropagation();
    }

}