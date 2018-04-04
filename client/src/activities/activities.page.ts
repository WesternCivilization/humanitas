import { Component, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';

import { Http, Response } from '@angular/http';
import { Breadcrumb } from '../core/breadcrumb';
import { IOption } from 'ng-select';
import { ZeteticaService } from '../providers/zetetica.service';
import { TimelineActivity } from '../providers/timelineactivity';
import { AppSettings } from '../core/app.settings';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

@Component({
    templateUrl: './activities.page.html'
})
export class ActivitiesPage implements OnInit, OnDestroy {

    showFilters: boolean = false;
    activities: Array<TimelineActivity>;
    activitiesCount: number;
    tag: string;
    sort: string = "4";
    isLoading: boolean;
    sub: Subscription;

    constructor(public _breadcrumb: Breadcrumb,
        public _settings: AppSettings,
        private _zeteticaService: ZeteticaService) {
        this.tag = AppSettings.FilterTag;
        this.sort = AppSettings.SortType;
    }

    ngOnInit(): void {
        this._breadcrumb.clear();
        this._breadcrumb.append("Atividades", "fa fa-calendar", "/activities");
        this.filter();
        //this.sub = Breadcrumb.filter.subscribe(options => this.filter(options));
    }

    ngOnDestroy(): void {
        if (this.sub) this.sub.unsubscribe();
    }

    setFilter(value: string): void {
        this.tag = value;
        this.filter();
    }

    setSort(value: string): void {
        this.sort = value;
        this.filter();
    }

    filter(): void {
        this.activities = new Array();
        this.isLoading = true;
        this._zeteticaService.activities(this.tag, this.sort, 0).subscribe(acts => {
            this.activities = this.activities.concat(acts.rows);
            this.isLoading = false;
            this.activitiesCount = acts.totalOfRecords;
        });
    }

    onScroll() {
        if (this.isLoading) return;
        this.isLoading = true;
        this._zeteticaService.activities(this.tag, this.sort, this.activities.length).subscribe(acts => {
            this.activities = this.activities.concat(acts.rows);
            this.activitiesCount = acts.totalOfRecords;
            this.isLoading = false;
        });
    }

}