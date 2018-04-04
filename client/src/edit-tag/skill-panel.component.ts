import { Component, Input } from '@angular/core';
import { TopicaService } from '../providers/topica.service';

@Component({
    templateUrl: './skill-panel.component.html',
    selector: "skill-panel"
})
export class SkillPanelComponent {

    @Input()
    public data:any;

    constructor(private _topicaService:TopicaService)
    {
    }

    save(data: any): void {
        this._topicaService.save(data).subscribe(result => this._topicaService.notifyAll(this.data));
    }

}