import { Component, Input } from '@angular/core';
import { AppSettings } from '../core/app.settings';
import { TopicaService } from '../providers/topica.service';
import { ZeteticaService } from '../providers/zetetica.service';

@Component({
    templateUrl: './references-panel.component.html',
    selector: "references-panel"
})
export class ReferencesPanelComponent {

    @Input()
    public data: any;

    constructor(public _settings: AppSettings,
        private _topicaService: TopicaService,
        private _zeteticaService: ZeteticaService) {
    }

    save(data: any): void {
        this._zeteticaService.save(data).subscribe(result => this._zeteticaService.notifyAll(this.data));
    }

}