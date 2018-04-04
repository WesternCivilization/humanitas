import { Component, Input } from '@angular/core';
import { AppSettings } from '../core/app.settings';
import { TopicaService } from '../providers/topica.service';

@Component({
    templateUrl: './tag-panel.component.html',
    selector: "tag-panel"
})
export class TagPanelComponent {

    @Input()
    public data:any;

    constructor(public _settings:AppSettings, 
        private _topicaService:TopicaService)
    {
    }

    save(data: any): void {
        this._topicaService.save(data).subscribe(result => this._topicaService.notifyAll(this.data));
    }

}