import { Component, ViewChild, OnInit, Output, Input, EventEmitter } from '@angular/core';

import { TopicaService } from '../providers/topica.service';
import { Subscription } from 'rxjs/Subscription';
import { debuglog } from 'util';
import { Player } from '../core/player';

@Component({
    templateUrl: './textarea.component.html',
    selector: "textarea-control"
})
export class TextAreaComponent implements OnInit {

    @Input("label")
    public label: string;

    @Input("value")
    public value: string = "";

    @Output("change")
    public change: EventEmitter<string> = new EventEmitter();

    constructor(public _player: Player) { }

    ngOnInit(): void { }

    setValue(value: any): void {
        this.value = value;
        this.change.emit(value);
    }

}