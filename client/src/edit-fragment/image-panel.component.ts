import { Component, Input } from '@angular/core';
import { ZeteticaService } from '../providers/zetetica.service';
import { AppSettings } from '../core/app.settings';

@Component({
    templateUrl: './image-panel.component.html',
    selector: "image-panel"
})
export class ImagePanelComponent {

    @Input()
    public data: any;

    imageUploading: boolean = false;

    constructor(private _zeteticaService: ZeteticaService,
        public _settings: AppSettings) {
    }

    save(data: any): void {
        this._zeteticaService.save(data).subscribe(result => this._zeteticaService.notifyAll(this.data));
    }

    setFileName(fileName: string): void {
        this.data.imageFileName = fileName;
        this.imageUploading = false;
    }

}