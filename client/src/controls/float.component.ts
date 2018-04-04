import { Component, ViewChild, OnInit, Output, Input, EventEmitter } from '@angular/core';

import { TopicaService } from '../providers/topica.service';
import { Subscription } from 'rxjs/Subscription';

@Component({
    templateUrl: './float.component.html',
    selector: "float-control"
})
export class FloatComponent implements OnInit {

    @Input("label")
    public label: string;

    @Input("value")
    public value: string = "";

    @Input("placeholder")
    public placeholder: string;

    @Input("icon")
    public icon: string;

    @Input()
    public type: string;

    wait: Subscription;
    options: Array<any>;

    constructor(private _topicaService: TopicaService) {
    }

    ngOnInit(): void { }

    query(ev: KeyboardEvent): void {
        if (ev.keyCode != 38 && ev.keyCode != 40 && ev.keyCode != 13) {
            if (this.wait) this.wait.unsubscribe();
            this.wait = this._topicaService.autocomplete(this.type, this.label)
                .subscribe((results) => {
                    this.options = results;
                });
        }
    }

}