import { Component, ViewChild, OnInit, OnChanges, Output, Input, EventEmitter } from '@angular/core';

import { TopicaService } from '../providers/topica.service';
import { Subscription } from 'rxjs/Subscription';

@Component({
    templateUrl: './preload.select.html',
    selector: "preload-select"
})
export class PreloadSelect implements OnInit, OnChanges {

    @Input("label")
    public label: string;

    @Input("value")
    public value: any = "";

    @Input("placeholder")
    public placeholder: string;

    @Input("icon")
    public icon: string;

    @Input()
    public type: string;

    @Output("select")
    public select: EventEmitter<any> = new EventEmitter();

    wait: Subscription;
    options: Array<any>;
    index: number = 0;
    selected: any;
    loading: boolean = false;

    constructor(private _topicaService: TopicaService) {
    }

    ngOnInit(): void {
        if (this.value) {
            if (this.wait) this.wait.unsubscribe();
            this.loading = true;
            this.wait = this._topicaService.select(this.type, this.value)
                .subscribe((option) => {
                    if (option != null) {
                        this.value = option.label + ' [' + option.value + ']';
                    }
                    else {
                        this.value = null;
                    }
                    this.loading = false;
                });
        }
    }

    ngOnChanges(): void {
        if (!this.loading && this.value.indexOf('[') == -1) {
            this.ngOnInit();
        }
    }

    query(ev: any): void {
        if (ev.keyCode != 38 && ev.keyCode != 40 && ev.keyCode != 13) {
            if (ev && ev.currentTarget && ev.currentTarget.value) {
                this.value = ev.currentTarget.value;
            }
            if (this.wait) this.wait.unsubscribe();
            this.loading = true;
            this.wait = this._topicaService.autocomplete(this.type, this.value)
                .subscribe((results) => {
                    this.options = results;
                    if (this.options.length > 0) {
                        this.index = 0;
                        this.options[this.index].selected = true;
                    }
                    this.loading = false;
                });
        }
        else if (ev.keyCode == 38 && this.options) {
            this.options[this.index].selected = false;
            this.index--;
            if (this.index < 0) this.index = 0;
            this.options[this.index].selected = true;
        }
        else if (ev.keyCode == 40 && this.options) {
            this.options[this.index].selected = false;
            this.index++;
            if (this.index >= this.options.length) this.index = this.options.length - 1;
            this.options[this.index].selected = true;
        }
        else if (ev.keyCode == 13 && this.options) {
            this.selected = { value: this.options[this.index].value, label: this.options[this.index].label };
            this.value = this.selected.value;
            this.select.emit({ value: this.options[this.index].value, label: this.options[this.index].label });
            this.options = null;
        }
        ev.stopPropagation();
    }

    onSelect(ev: any): void {
        this.options.forEach(element => {
            if (element.selected) {
                this.selected = { value: element.value, label: element.label };
                this.value = element.value;
                this.select.emit({ value: element.value, label: element.label });
                this.options = null;
            }
        });
        ev.stopPropagation();
    }

    setValue(ev: any): void {
        if (ev && ev.currentTarget && ev.currentTarget.value) {
            this.value = ev.currentTarget.value;
        }
        ev.stopPropagation();
    }

}