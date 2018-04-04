import { Component, ViewChild, OnInit, Output, Input, EventEmitter } from '@angular/core';

import { IOption, SelectComponent } from 'ng-select';
import { TopicaService } from '../providers/topica.service';
import { debug, debuglog } from 'util';
import { Subscription } from 'rxjs/Subscription';

@Component({
    templateUrl: './tagselect.component.html',
    selector: "tagselect"
})
export class TagSelectComponent implements OnInit {

    showFilters: boolean = false;
    options: Array<IOption> = [];
    wait:Subscription;

    @Output()
    public select:EventEmitter<Array<IOption>> = new EventEmitter();

    @Input()
    public type: string;

    public selecteds: Array<IOption> = [];

    constructor(private _topicaService: TopicaService) {
    }

    @ViewChild(SelectComponent) filter: SelectComponent;

    ngOnInit(): void { }

    selected(option: IOption) {
        this.selecteds.push(option);
        this.select.emit(this.selecteds);
    }

    deselected(option: IOption) {
        this.selecteds = this.selecteds.filter(el => el.value != option.value);
        this.select.emit(this.selecteds);
    }

    query(ev: KeyboardEvent): void {
        if (ev.keyCode != 38 && ev.keyCode != 40 && ev.keyCode != 13) {
            var text = this.filter.filterInput.nativeElement.value;
            
            if(this.wait) this.wait.unsubscribe();
            this.wait = this._topicaService.autocomplete(this.type, text)
                            .subscribe((results) => {
                                this.options = results;
                                this.options = this.options.concat(this.selecteds);
                            });
        }
    }

}