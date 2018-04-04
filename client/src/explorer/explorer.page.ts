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
import { DomSanitizer } from '@angular/platform-browser';

@Component({
    templateUrl: './explorer.page.html'
})
export class ExplorerPage implements OnInit {

    detail: any;

    detailLoading: boolean;
    foldersLoading: boolean;
    fragmentsLoading: boolean;
    fragmentsCount: number;
    fragmentsScroll: boolean;
    booksLoading: boolean;
    booksCount: number;

    folders: Array<any> = [];
    fragments: Array<TimelineActivity> = [];
    books: Array<TimelineActivity> = [];

    public domainId: number;

    public parent: string;

    public parentName: string;

    public parentType: string;

    public title: string;
    public icon: string;

    constructor(public _settings: AppSettings,
        private _route: ActivatedRoute,
        public _breadcrumb: Breadcrumb,
        private _topicaService: TopicaService,
        private _zeteticaService: ZeteticaService,
        private _domSanitizer: DomSanitizer) {
    }

    ngOnInit(): void {

        this.title = "Filosofias";
        this.icon = "fa fa-university";
        this._route.params.subscribe(params => {
            if (!params["domainId"] && !params["parent"]) {
                this._breadcrumb.clear();
                this._breadcrumb.append("Filosofias", "fa fa-university", "/explorer");
                this.title = "Filosofias";
                this.icon = "fa fa-university";
            }
            if (params["domainId"] && !params["parent"] && this._breadcrumb.getPath().length >= 1) {
                let url = "/explorer;domainId=" + params["domainId"];
                let name = this._settings.getDomainName(+params["domainId"]);
                let icon = this._settings.getEntityIcon("domain-" + params["domainId"]);
                this._breadcrumb.append(name, icon, url);
                this.title = name;
                this.icon = icon;
            }
            this.domainId = +params["domainId"];
            if (this.parent != params["parent"]) {
                this.parent = params["parent"];
                this.parentName = params["parentName"];
                this.parentType = params["parentType"];
                this.title = this.parentName;
                if (this.parent && this.parentName) {
                    this.fragmentsLoading = true;
                    this.fragments = new Array();
                    this.foldersLoading = true;
                    this.folders = new Array();
                    this.booksLoading = true;
                    this.books = new Array();

                    let name = this.parentName;
                    let icon = this._settings.getEntityIcon(this.parentType);
                    this.icon = icon;
                    this._breadcrumb.append(name, icon, "/explorer;domainId=" + this.domainId + "&parent=" + this.parent + "&parentName=" + this.parentName + "&parentType=" + this.parentType);
                    this.detailLoading = true;
                    this._topicaService.detail(this.parent).subscribe(detail => {
                        this.detail = detail;
                        this.detailLoading = false;
                    });
                }
                else {
                    this.detail = null;
                }
            }
            this.loadChildren();
        });
    }

    loadChildren(): void {

        this.folders = new Array();
        if (!this.domainId && !this.parent) {
            this.folders = new Array();
            this.folders.push({
                value: 18,
                icon: this._settings.getEntityIcon("domain-18"),
                label: "Filosofia da Alma",
                link: "/explorer;domainId=18"
            });
            this.folders.push({
                value: 20,
                icon: this._settings.getEntityIcon("domain-20"),
                label: "Filosofia do Entendimento",
                link: "/explorer;domainId=20"
            });
            this.folders.push({
                value: 19,
                icon: this._settings.getEntityIcon("domain-19"),
                label: "Filosofia da Consciência",
                link: "/explorer;domainId=19"
            });
            this.folders.push({
                value: 15,
                icon: this._settings.getEntityIcon("domain-15"),
                label: "Filosofia dos Costumes",
                link: "/explorer;domainId=15"
            });
            this.folders.push({
                value: 17,
                icon: this._settings.getEntityIcon("domain-17"),
                label: "Filosofia do Estado",
                link: "/explorer;domainId=17"
            });
            this.folders.push({
                value: 16,
                icon: this._settings.getEntityIcon("domain-16"),
                label: "Filosofia da Cultura",
                link: "/explorer;domainId=16"
            });
            this.folders.push({
                value: 13,
                icon: this._settings.getEntityIcon("domain-13"),
                label: "Filosofia da Economia",
                link: "/explorer;domainId=13"
            });
            this.folders.push({
                value: 14,
                icon: this._settings.getEntityIcon("domain-14"),
                label: "Filosofia da Tecnologia",
                link: "/explorer;domainId=14"
            });
            this.folders.push({
                value: 12,
                icon: this._settings.getEntityIcon("domain-12"),
                label: "Filosofia das Nações",
                link: "/explorer;domainId=12"
            });
            this.folders.push({
                value: 10,
                icon: this._settings.getEntityIcon("domain-10"),
                label: "Filosofia da Música",
                link: "/explorer;domainId=10"
            });
            this.folders.push({
                value: 11,
                icon: this._settings.getEntityIcon("domain-11"),
                label: "Filosofia da Matemática",
                link: "/explorer;domainId=11"
            });
            this.folders.push({
                value: 9,
                icon: this._settings.getEntityIcon("domain-9"),
                label: "Filosofia dos Princípios",
                link: "/explorer;domainId=9"
            });

            this.folders.forEach(element => {
                if (element.link.indexOf(';') != -1) {
                    let queryParams = {};
                    let pars = element.link.split(';')[1].split('&');
                    pars.forEach(element2 => {
                        let keyValue = element2.split('=');
                        queryParams[keyValue[0]] = keyValue[1];
                    });
                    element.queryParams = queryParams;
                    element.link = element.link.split(';')[0];
                }
            });
        }
        else {

            this.foldersLoading = true;
            this._topicaService.folders(this.domainId > 0 ? this.domainId : 41, this.parent)
                .subscribe(folders => {
                    this.folders = folders;
                    this.foldersLoading = false;
                    this.folders.forEach(element => {
                        element.link = "/explorer;domainId=" + this.domainId + "&parent=" + element.value + "&parentName=" + element.label + "&parentType=" + element.type;
                        if (element.link.indexOf(';') != -1) {
                            let queryParams = {};
                            let pars = element.link.split(';')[1].split('&');
                            pars.forEach(element2 => {
                                let keyValue = element2.split('=');
                                queryParams[keyValue[0]] = keyValue[1];
                            });
                            element.queryParams = queryParams;
                            element.link = element.link.split(';')[0];
                            element.icon = this._settings.getEntityIcon(element.type);
                        }
                    });
                });

            this.fragmentsLoading = true;
            this._zeteticaService.fragments(this.domainId ? this.domainId : 41, this.parent, this.fragments.length)
                .subscribe(fragments => {
                    this.fragments = new Array().concat(fragments.rows);
                    this.fragmentsCount = fragments.totalOfRecords;
                    this.fragmentsLoading = false;
                });

            this.booksLoading = true;
            this._topicaService.books(null, [this.parent], 0)
                .subscribe(books => {
                    this.books = new Array().concat(books.rows);
                    this.booksCount = books.totalOfRecords;
                    this.booksLoading = false;
                });

        }
    }

    url(src:string){
        return this._domSanitizer.bypassSecurityTrustStyle(src);
    }

    trunc(text: string, length: number): string {
        if (text.length > length)
            return text.substring(0, length) + "..";
        else
            return text;
    }

    public showDate(event: any): string {
        if (event && event.date) {
            return event.date.split('T')[0];
        }
        else if (event && event.century) {
            if (event.century > 0) {
                return "Século " + Math.abs(event.century) + " d.C.";
            }
            else {
                return "Século " + Math.abs(event.century) + " a.C.";
            }
        }
        else if (event && event.year) {
            if (event.year > 0) {
                return Math.abs(event.year) + " d.C.";
            }
            else {
                return Math.abs(event.year) + " a.C.";
            }
        }
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



    onFragmentsScroll() {
        if (this.fragmentsScroll) return;
        this.fragmentsScroll = true;
        this._zeteticaService.fragments(this.domainId ? this.domainId : 41, this.parent, this.fragments.length)
            .subscribe(fragments => {
                this.fragments = this.fragments.concat(fragments.rows);
                this.fragmentsCount = fragments.totalOfRecords;
                this.fragmentsScroll = false;
                this.fragmentsLoading = false;
            });

    }

}