import { Component, Input } from '@angular/core';
import { TopicaService } from '../providers/topica.service';
import { AppSettings } from '../core/app.settings';

@Component({
    templateUrl: './image-panel.component.html',
    selector: "image-panel"
})
export class ImagePanelComponent {

    @Input()
    public data: any;

    uploading: boolean = false;

    constructor(private _topicaService: TopicaService,
        public _settings: AppSettings) {
    }

    save(data: any): void {
        this._topicaService.save(data).subscribe(result => this._topicaService.notifyAll(this.data));
    }

    setFileName(fileName: string): void {
        this.data.fileName = fileName;
        this.uploading = false;
    }

}