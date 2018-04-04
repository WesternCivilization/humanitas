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
import { TerminalService } from '../providers/terminal.service';

@Component({
    templateUrl: './terminal.page.html'
})
export class TerminalPage {

    sub: Subscription;
    commands: string;
    results: string;
    isRunning: boolean = false;

    constructor(public _breadcrumb: Breadcrumb,
        private _terminalService: TerminalService) {
    }

    ngOnInit(): void {
        this._breadcrumb.clear();
        this._breadcrumb.append("Terminal", "fa fa-terminal", "/terminal");
        this.commands = `// Adicionar citação
//Título:=Das razões verdadeiras
//Tipo:=Citação
//Texto:=Razões verdadeiras são aquelas que os legisladores mais sábios seguiram; e elas provêm da lei da natureza ou da consideração do estado.
//Autor:=Leibniz
//Livro:=arte das controvérsias
//Person:=leibniz
//Página:=130

// Adicionar nota
//Título:=Da presunção católica
//Tipo:=Nota
//Texto:=O problema de o sujeito decorar a Doutrina da Igreja é que ele fica se achando preparado para julgar todas as situações. O diabo adora isso.
//Autor:=Olavo de Carvalho
//Person:=Olavo de Carvalho
//Referência:=Facebook

// Adicionar tag
//Nome:=Aristóteles
//Tipo:=autor
//Nascimento:=-384, Estagira
//Morte:=-318, Atenas, Grécia`;
    }

    run(): void {
        this.isRunning = true;
        this._terminalService.run({ Content: this.commands })
            .subscribe(results => { 
                this.results = results.html;
                this.isRunning = false; 
            });
        this.commands = "";
    }

}