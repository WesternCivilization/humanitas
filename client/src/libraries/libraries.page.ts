import { Component, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { Http, Response } from '@angular/http';
import { Breadcrumb } from '../core/breadcrumb';
import { IOption } from 'ng-select';
import { TimelineActivity } from '../providers/timelineactivity';
import { AppSettings } from '../core/app.settings';
import { TopicaService } from '../providers/topica.service';
import { DomSanitizer } from '@angular/platform-browser';
import { Subscription } from 'rxjs/Subscription';

@Component({
    templateUrl: './libraries.page.html'
})
export class LibrariesPage implements OnInit, OnDestroy {

    books: Array<any>;
    booksCount: number;
    tags: Array<string>;
    isLoading: boolean;
    libraryId: string;
    libraryName: string;
    sub: Subscription;

    constructor(public _breadcrumb: Breadcrumb,
        public _settings: AppSettings,
        private _route: ActivatedRoute,
        private _topicaService: TopicaService,
        private _domSanitizer: DomSanitizer) {
    }

    ngOnInit(): void {
        this._route.params.subscribe(params => {
            this.libraryId = params["libraryId"];
            this.libraryName = params["libraryName"];
            this.tags = new Array();
            this._breadcrumb.clear();
            this._breadcrumb.append("Biblioteca Virtual", "fa fa-book", "/libraries");
            if (!this.libraryId) {
                this.libraries();
            }
            else {
                this._breadcrumb.append(this.libraryName, "fa fa-book", "/libraries;libraryId=" + this.libraryId + "&libraryName=" + this.libraryName);
                this.filter(new Array());
            }
        });

        this.sub = Breadcrumb.filter.subscribe(options => this.filter(options));
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

    libraries(): any {
        this.books = new Array();
        this._topicaService.libraries().subscribe(books => {
            this.books = books;
        });
    }

    filter(ev: Array<IOption>): void {
        this.tags = new Array();
        ev.forEach(item => this.tags.push(item.value));
        this.books = new Array();
        this._topicaService.books(this.libraryId, this.tags, 0).subscribe(books => {
            this.books = this.books.concat(books.rows);
            this.booksCount = books.totalOfRecords;
        });
    }

    url(src:string){
        return this._domSanitizer.bypassSecurityTrustStyle(src);
    }

    onScroll() {
        if (this.isLoading) return;
        this.isLoading = true;
        this._topicaService.books(this.libraryId, this.tags, this.books.length).subscribe(books => {
            this.books = this.books.concat(books.rows);
            this.booksCount = books.totalOfRecords;
            this.isLoading = false;
        });
    }

}