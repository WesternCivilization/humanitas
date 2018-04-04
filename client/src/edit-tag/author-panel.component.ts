import { Component, Input } from '@angular/core';
import { TopicaService } from '../providers/topica.service';

@Component({
    templateUrl: './author-panel.component.html',
    selector: "author-panel"
})
export class AuthorPanelComponent {

    @Input()
    public data: any;

    constructor(private _topicaService:TopicaService)
    {
    }

    save(data: any): void {
        this._topicaService.save(data).subscribe(result => this._topicaService.notifyAll(this.data));
    }

    setBeginDate(value: string): void {
        let parts = value.split('|');
        this.data.beginDate = parts[0];
        this.data.beginYear = parts[1];
        this.data.beginCentury = parts[2];
    }

    setEndDate(value: string): void {
        let parts = value.split('|');
        this.data.endDate = parts[0];
        this.data.endYear = parts[1];
        this.data.endCentury = parts[2];
    }

}