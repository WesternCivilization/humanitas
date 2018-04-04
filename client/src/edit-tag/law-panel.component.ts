import { Component, Input } from '@angular/core';
import { TopicaService } from '../providers/topica.service';

@Component({
    templateUrl: './law-panel.component.html',
    selector: "law-panel"
})
export class LawPanelComponent {

    @Input()
    public data:any;

    constructor(private _topicaService:TopicaService)
    {
    }

    save(data: any): void {
        this._topicaService.save(data).subscribe(result => this._topicaService.notifyAll(this.data));
    }


}