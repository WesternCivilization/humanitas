import { Component, Input } from '@angular/core';
import { TopicaService } from '../providers/topica.service';

@Component({
    templateUrl: './links-panel.component.html',
    selector: "links-panel"
})
export class LinksPanelComponent {

    @Input()
    public data: any;

    public selected: any;

    constructor(private _topicaService:TopicaService)
    {
    }

    save(data: any): void {
        this._topicaService.save(data).subscribe(result => this._topicaService.notifyAll(this.data));
    }

    remove(event: any): void {
        let arr = new Array();
        this.data.links.forEach(item => {
            if (item != event) {
                arr.push(item);
            }
        });
        this.data.links = arr;
    }

    saveEvent(): void {
        this.selected = null;
    }

    add(): void {
        if (!this.data.links) {
            this.data.links = new Array();
        }
        this.selected = { label: null, url: null };
        this.data.links.push(this.selected);
    }

}