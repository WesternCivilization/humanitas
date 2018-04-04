import { Component, ViewChild, OnInit, Output, Input, EventEmitter } from '@angular/core';

@Component({
    templateUrl: './text.component.html',
    selector: "text-control"
})
export class TextComponent implements OnInit {

    @Input("label")
    public label: string;

    @Input("value")
    public value: string = "";

    @Input("placeholder")
    public placeholder: string;

    @Input("icon")
    public icon: string;

    @Output("change")
    public change: EventEmitter<string> = new EventEmitter();

    constructor() {
    }

    ngOnInit(): void { }

    setValue(ev: any): void {
        if (ev && ev.currentTarget && ev.currentTarget.value) {
            this.value = ev.currentTarget.value;
            this.change.emit(this.value);
        }
        ev.stopPropagation();
}

}