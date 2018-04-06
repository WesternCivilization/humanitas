import { Component, ViewChild, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { Breadcrumb } from '../core/breadcrumb';

import { Http, Response } from '@angular/http';
import 'rxjs/add/operator/map';
import { AppSettings } from '../core/app.settings';
import { TopicaService } from '../providers/topica.service';
import { ZeteticaService } from '../providers/zetetica.service';

@Component({
    templateUrl: './edit-fragment.page.html'
})
export class EditFragmentPage implements OnInit {

    activePanel: string = "fragment";
    id: string;
    detail: any;
    type: string = "none";
    detailLoading: boolean;
    saved: any;

    constructor(public _breadcrumb: Breadcrumb,
        public _settings: AppSettings,
        private _route: ActivatedRoute,
        private _router: Router,
        private _topicaService: TopicaService,
        private _zeteticaService: ZeteticaService) {
        ZeteticaService.saved.subscribe(data => this.saved = data);
        console.warn("constructor");
    }

    ngOnInit(): void {
        console.warn("init");
        if (this._settings.getUserType() > 0) {
            alert("Acesso negado. Você não é administrador.");
            this._router.navigate(['/activities']);
        }
        this._route.params.subscribe(params => {
            if (this.id != params["id"]) {
                this.id = params["id"];
                this.detailLoading = true;
                this._zeteticaService.detail(this.id).subscribe(detail => {
                    this.detail = detail;
                    this.type = this._settings.getEntityType('fragment', detail.type);
                    let icon = "fa fa-edit";
                    this._breadcrumb.append("Editando " + this._settings.trunc(detail.title, 10), icon, "/edit-fragment/" + this.detail.fragmentId.toUpperCase());
                    this.detailLoading = false;
                });
            }
        });
    }

    setPanel(name: string): void {
        this.activePanel = name;
    }

}