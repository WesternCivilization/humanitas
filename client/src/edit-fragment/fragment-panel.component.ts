import { Component, Input } from '@angular/core';
import { AppSettings } from '../core/app.settings';
import { ZeteticaService } from '../providers/zetetica.service';
import { Player } from '../core/player';

@Component({
    templateUrl: './fragment-panel.component.html',
    selector: "fragment-panel"
})
export class FragmentPanelComponent {

    @Input()
    public data: any;

    constructor(public _settings: AppSettings,
        private _zeteticaService: ZeteticaService,
        public _player: Player) {
    }

    save(data: any): void {
        this._zeteticaService.save(data).subscribe(result => this._zeteticaService.notifyAll(this.data));
    }

}