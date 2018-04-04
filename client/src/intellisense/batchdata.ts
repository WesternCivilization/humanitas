import { Component, Input, Output, EventEmitter, ElementRef, ViewChild, NgZone } from '@angular/core';
import { AppSettings } from '../core/app.settings';
import { TopicaService } from '../providers/topica.service';
import { Subscription } from 'rxjs/Subscription';
import { debuglog } from 'util';
import { ZeteticaService } from '../providers/zetetica.service';

@Component({
    templateUrl: './batchdata.html',
    selector: "batchdata"
})
export class BatchData {

    @ViewChild("el")
    el: ElementRef;

    @Input()
    content: string;

    @Output()
    change: EventEmitter<string> = new EventEmitter();

    container: HTMLDivElement;
    select: HTMLSelectElement;
    sub: Subscription;
    type: string;
    index: number;

    quoteFields: string = "@Citação>Título:=|TipoFragmento:=Citação|Texto:=|Autor:=|Domínio:=|Dentro de:=|Pessoas:=|Tópicos:=|Livro:=|Página:=|Referência:=";
    noteFields: string = "@Nota>Título:=|TipoFragmento:=Nota|Texto:=|Autor:=|Domínio:=|Dentro de:=|Pessoas:=|Tópicos:=|Referência:=";
    authorFields: string = "@Autor>Nome:=|TipoTag:=Autor|Texto:=|Nascimento:=|Morte:=|Domínio:=|Dentro de:=|Pessoas:=|Tópicos:=";
    bookFields: string = "@Livro>Nome:=|TipoTag:=Livro|Texto:=|Edição:=|Isbn:=|Editora:=|Ano:=|Tradutor:=|Comprado em:=|Preço:=|Domínio:=|Dentro de:=|Pessoas:=|Tópicos:=";

    constructor(public _settings: AppSettings,
        public _topicaService: TopicaService,
        public _zeteticaService: ZeteticaService,
        private zone: NgZone) {
    }

    addTemplate(range: any, fields: Array<string>, ev: any): void {
        range.endContainer.textContent = fields[0];
        range.setStart(range.endContainer, range.endContainer.textContent.length);
        for (let i = 1; i < fields.length; i++) {
            let p = document.createElement("p");
            p.innerText = fields[i];
            range.endContainer.parentElement.appendChild(p);
        }
        ev.preventDefault();
    }

    keydown(ev: any): void {
        if (ev.keyCode == 27) {
            this.unselect();
            return;
        }
        if (this.select) {
            if (ev.keyCode == 38) {
                ev.preventDefault();
            }
            else if (ev.keyCode == 40) {
                ev.preventDefault();
            }
            else if (ev.keyCode == 13) {
                ev.preventDefault();
            }
        }
        else if (ev.keyCode == 13) {
            this.el.nativeElement.focus();
            let _range = document.getSelection().getRangeAt(0);
            let txt = _range.endContainer.textContent;

            if (txt && txt.length > 0) {
                if (this.quoteFields.startsWith(txt)) {
                    this.addTemplate(_range, this.quoteFields.substring(this.quoteFields.indexOf('>') + 1).split('|'), ev);
                }
                else if (this.noteFields.startsWith(txt)) {
                    this.addTemplate(_range, this.noteFields.substring(this.noteFields.indexOf('>') + 1).split('|'), ev);
                }
                else if (this.authorFields.startsWith(txt)) {
                    this.addTemplate(_range, this.authorFields.substring(this.authorFields.indexOf('>') + 1).split('|'), ev);
                }
                else if (this.bookFields.startsWith(txt)) {
                    this.addTemplate(_range, this.bookFields.substring(this.bookFields.indexOf('>') + 1).split('|'), ev);
                }
            }
        }

    }

    keyup(ev: any): void {
        this.content = this.el.nativeElement.innerText;
        this.change.emit(this.content);
        if (ev.keyCode == 38) {
            if (!this.select) return;
            this.refreshSelection(this.index - 1);
            ev.preventDefault();
            return;
        }
        else if (ev.keyCode == 40) {
            if (!this.select) return;
            this.refreshSelection(this.index + 1);
            ev.preventDefault();
            return;
        }
        else if (ev.keyCode == 13) {
            if (this.select) {
                this.selectItem(this.index);
            }
            return;
        }
        else if (ev.ctrlKey || ev.keyCode == 39) {
            this.unselect();
        }

        this.el.nativeElement.focus();
        let _range = document.getSelection().getRangeAt(0);
        let txt = _range.endContainer.textContent;
        let isTag = false;
        if (txt.startsWith("Pessoas:=")) {
            this.type = "author";
            isTag = true;
            txt = txt.substring(9);
        }
        else if (txt.startsWith("Dentro de:=")) {
            this.type = "tag";
            isTag = true;
            txt = txt.substring(11);
        }
        else if (txt.startsWith("@Citações:=")) {
            this.type = "tag";
            isTag = true;
            txt = txt.substring(11);
        }
        else if (txt.startsWith("@Notas:=")) {
            this.type = "tag";
            isTag = true;
            txt = txt.substring(8);
        }
        else if (txt.startsWith("Tópicos:=")) {
            this.type = "topic";
            isTag = true;
            txt = txt.substring(9);
        }
        else if (txt.startsWith("Área:=")) {
            this.type = "area";
            isTag = true;
            txt = txt.substring(6);
        }
        else if (txt.startsWith("Período:=")) {
            this.type = "period";
            isTag = true;
            txt = txt.substring(9);
        }
        else if (txt.startsWith("Habilidades:=")) {
            this.type = "skill";
            isTag = true;
            txt = txt.substring(13);
        }
        else if (txt.startsWith("Domínio:=")) {
            this.type = "domain";
            isTag = true;
            txt = txt.substring(9);
        }
        else if (txt.startsWith("Livro:=")) {
            this.type = "book";
            isTag = true;
            txt = txt.substring(7);
        }
        else if (txt.startsWith("Instituições:=")) {
            this.type = "institution";
            isTag = true;
            txt = txt.substring(14);
        }
        else if (txt.startsWith("TipoFragmento:=")) {
            this.type = "fragmenttype";
            isTag = true;
            txt = txt.substring(15);
        }
        else if (txt.startsWith("TipoTag:=")) {
            this.type = "tagtype";
            isTag = true;
            txt = txt.substring(9);
        }
        if (isTag) {
            let parts = txt.split(';');
            txt = parts[parts.length - 1];
            if (txt.length > 0) {
                if (!this.select) {
                    this.create(txt);
                }
                this.query(txt);
            }
            else {
                this.unselect();
            }
        }
    }

    query(expression: string): any {
        if (this.sub) this.sub.unsubscribe();

        this.sub = this._topicaService.autocomplete(this.type, expression)
            .subscribe(options => {
                this.select.options.length = 0;
                options.forEach(element => {
                    let opt = document.createElement("option");
                    opt.text = element.label;
                    opt.value = element.value;
                    this.select.appendChild(opt);
                });
                if (options.length > 0) {
                    this.index = 0;
                    this.select.options[0].selected = true;
                }
            });
    }

    create(expression: string): any {
        let sel = window.getSelection();
        if (sel.getRangeAt && sel.rangeCount) {
            let range = sel.getRangeAt(0);
            let clientPos = range.getClientRects();
            range.deleteContents();

            // Range.createContextualFragment() would be useful here but is
            // non-standard and not supported in all browsers (IE9, for one)
            this.container = document.createElement("div");
            this.select = document.createElement("select");
            this.select.multiple = true;
            this.select.style["position"] = "absolute";
            let y = (<any>clientPos.item(0)).y + 30;
            let x = (<any>clientPos.item(0)).x;
            this.select.style["top"] = y + "px";
            this.select.style["left"] = x + "px";
            this.container.appendChild(this.select);
            document.body.appendChild(this.container);
        }

    }

    refreshSelection(index: number): void {
        let previous = this.index;
        let next = index;
        if (next < 0) return;
        if (next >= this.select.options.length) return;
        this.select.options[previous].selected = false;
        this.select.options[next].selected = true;
        this.index = next;
    }

    selectItem(index: number): void {
        let label = this.select.options[index].label;

        this.el.nativeElement.focus();
        let _range = document.getSelection().getRangeAt(0);
        let txt = _range.endContainer.textContent;

        _range.endContainer.textContent = this.replaceValue(txt, label);
        _range.setStart(_range.endContainer, _range.endContainer.textContent.length);

        if (_range.endContainer.textContent.startsWith("@Notas:=")) {
            let name = _range.endContainer.textContent.substring(_range.endContainer.textContent.indexOf('=') + 1);
            _range.endContainer.textContent = "Carregando Notas...";

            if (this.sub) this.sub.unsubscribe();
            this.sub = this._zeteticaService.batchdata("note", name)
                .subscribe(data => {
                    _range.endContainer.textContent = "";
                    data.lines.forEach(line => {
                        let p = document.createElement("div");
                        p.style['margin'] = '0';
                        p.innerText = line;
                        _range.endContainer.parentElement.appendChild(p);
                    });
                });
        }
        else if (_range.endContainer.textContent.startsWith("@Citações:=")) {
            let name = _range.endContainer.textContent.substring(_range.endContainer.textContent.indexOf('=') + 1);
            _range.endContainer.textContent = "Carregando Citações...";

            if (this.sub) this.sub.unsubscribe();
            this.sub = this._zeteticaService.batchdata("quote", name)
                .subscribe(data => {
                    _range.endContainer.textContent = "";
                    data.lines.forEach(line => {
                        let p = document.createElement("div");
                        p.style['margin'] = '0';
                        p.innerText = line;
                        _range.endContainer.parentElement.appendChild(p);
                    });
                });
        }

        this.container.remove();
        this.container = null;
        this.select = null;
    }

    replaceValue(original: string, option: string): string {
        let keep = original.lastIndexOf(';') > 0 ? original.substring(original.indexOf('=') + 1, original.lastIndexOf(';') + 1) : '';
        let result = original.substring(0, original.indexOf('=') + 1) + keep + option;
        return result;
    }

    unselect(): void {
        if (this.container) {
            this.container.remove();
            this.container = null;
            this.select = null;
        }
    }

}