import { Component, Input, Output, EventEmitter } from '@angular/core';
import { AppSettings } from '../core/app.settings';

@Component({
    templateUrl: './topicreorg.html',
    selector: "topicreorg"
})
export class TopicReorg {

    constructor(public _settings: AppSettings) {
    }

    keyup(ev: any): void {
        console.warn(ev);
    }

}