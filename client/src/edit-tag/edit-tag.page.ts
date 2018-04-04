import { Component, ViewChild, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { Breadcrumb } from '../core/breadcrumb';

import { Http, Response } from '@angular/http';
import 'rxjs/add/operator/map';
import { AppSettings } from '../core/app.settings';
import { TopicaService } from '../providers/topica.service';
import { ZeteticaService } from '../providers/zetetica.service';

@Component({
    templateUrl: './edit-tag.page.html'
})
export class EditTagPage implements OnInit {

    activePanel: string = "tag";
    id: string;
    detail: any;
    type: string = "none";
    detailLoading: boolean;
    fragments: Array<any>;
    fragmentsCount: number;
    fragmentsScroll: boolean;
    saved: any;

    constructor(public _breadcrumb: Breadcrumb,
        public _settings: AppSettings,
        private _route: ActivatedRoute,
        private _router: Router,
        private _topicaService: TopicaService,
        private _zeteticaService: ZeteticaService) {
        TopicaService.saved.subscribe(data => this.saved = data);
    }

    ngOnInit(): void {
        if (this._settings.getUserType() > 0) {
            alert("Acesso negado. Você não é administrador.");
            this._router.navigate(['/activities']);
        }
        this._route.params.subscribe(params => {
            if (this.id != params["id"]) {
                this.id = params["id"];
                this.detailLoading = true;
                this._topicaService.detail(this.id).subscribe(detail => {
                    this.detail = detail;
                    this.type = this._settings.getEntityType('tag', detail.type);
                    let icon = "fa fa-edit";
                    this._breadcrumb.append("Editando " + this._settings.trunc(detail.name, 10), icon, "/edit-tag/" + this.detail.tagId.toUpperCase());
                    this.detailLoading = false;

                    if (this.type == "book") {
                        this._topicaService.libraryBook(this.id, "").subscribe(book => {
                            this.detail.libraryBook = book;
                        })
                    }

                    this._zeteticaService.fragments(this.detail.domainId ? this.detail.domainId : 41, this.detail.tagId, 0).subscribe(acts => {
                        this.fragments = acts.rows;
                        this.fragmentsCount = acts.totalOfRecords;
                    });
                });
            }
        });
    }

    setPanel(name: string): void {
        this.activePanel = name;
    }

    onFragmentsScroll() {
        if (this.fragmentsScroll) return;
        this.fragmentsScroll = true;
        this._zeteticaService.fragments(41, this.id, this.fragments.length)
            .subscribe(fragments => {
                this.fragments = this.fragments.concat(fragments.rows);
                this.fragmentsCount = fragments.totalOfRecords;
                this.fragmentsScroll = false;
            });

    }


}