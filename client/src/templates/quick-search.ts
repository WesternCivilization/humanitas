import { Component, Output, Input, OnInit, EventEmitter } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { TopicaService } from '../providers/topica.service';
import { Router } from '@angular/router';
import { AppSettings } from '../core/app.settings';

@Component({
    templateUrl: './quick-search.html',
    selector: "quick-search"
})
export class QuickSearch implements OnInit {

    @Input()
    public data: Array<any>;

    public selected: any;

    value: string;
    wait: Subscription;
    options: Array<any>;
    index: number = 0;
    loading: boolean = false;
    inputSearch: any;

    constructor(private _topicaService: TopicaService,
        private _settings: AppSettings,
        private _router: Router) {
    }

    ngOnInit(): void {
    }

    search(ev: any): void {
        if (this.wait) this.wait.unsubscribe();
        if (this.inputSearch && this.selected) {
            let length = this.inputSearch.selectionStart;
            this.inputSearch.setSelectionRange(length, this.selected.label.length);
        }
        if (ev.keyCode != 38 && ev.keyCode != 40 && ev.keyCode != 13) {
            if (ev && ev.currentTarget && ev.currentTarget.value) {
                this.value = ev.currentTarget.value;
                this.inputSearch = ev.currentTarget;
            }
            this.loading = true;
            this.wait = this._topicaService.autocomplete("all", this.value)
                .subscribe((results) => {
                    if (results && results.length > 0) {
                        if (this.inputSearch) {
                            this.value = this.inputSearch.value;
                            let length = this.inputSearch.selectionStart;
                            if (length > 0) {
                                let previous = this.value.toLowerCase();
                                let next = results[0].label.toLowerCase();
                                if (next.startsWith(previous)) {
                                    this.selected = results[0];
                                    this.value = results[0].label.substr(0, length) + results[0].label.substr(length);
                                    this.inputSearch.value = this.value;
                                    this.inputSearch.setSelectionRange(length, this.value.length);
                                }
                                else {
                                    this.selected = null;
                                }
                            }
                        }

                    }
                    this.loading = false;
                });
        }
        else if (ev.keyCode == 38 && this.options) {
            this.options[this.index].selected = false;
            this.index--;
            if (this.index < 0) this.index = 0;
            this.options[this.index].selected = true;
        }
        else if (ev.keyCode == 40 && this.options) {
            this.options[this.index].selected = false;
            this.index++;
            if (this.index >= this.options.length) this.index = this.options.length - 1;
            this.options[this.index].selected = true;
        }
        ev.stopPropagation();
    }

    tryRedirect(ev: any): boolean {
        if (ev.keyCode == 13 && this.selected && this.selected.type) {
            let type = this._settings.getEntityType("tag", +this.selected.type);
            this._router.navigate(["/explorer", { "domainId": 41, "parent": this.selected.value, "parentName": this.selected.label, "parentType": type }]);
            return false;
        }
        else {
            return true;
        }
    }


}