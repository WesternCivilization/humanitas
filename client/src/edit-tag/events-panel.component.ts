import { Component, Input } from '@angular/core';
import { TopicaService } from '../providers/topica.service';

@Component({
    templateUrl: './events-panel.component.html',
    selector: "events-panel"
})
export class EventsPanelComponent {

    @Input()
    public data: any;

    public selected: any;

    constructor(private _topicaService: TopicaService) {
    }

    save(data: any): void {
        this._topicaService.save(data).subscribe(result => this._topicaService.notifyAll(this.data));
    }

    public showDate(event: any): string {
        if (event && event.date) {
            return event.date.split('T')[0];
        }
        else if (event && event.century) {
            if (event.century > 0) {
                return "Século " + Math.abs(event.century) + " d.C.";
            }
            else {
                return "Século " + Math.abs(event.century) + " a.C.";
            }
        }
        else if (event && event.year) {
            if (event.year > 0) {
                return Math.abs(event.year) + " d.C.";
            }
            else {
                return Math.abs(event.year) + " a.C.";
            }
        }
    }

    setDate(value: string): void {
        let parts = value.split('|');
        this.selected.date = parts[0];
        this.selected.year = parts[1];
        this.selected.century = parts[2];
    }

    remove(event: any): void {
        let arr = new Array();
        this.data.events.forEach(item => {
            if (item != event) {
                arr.push(item);
            }
        });
        this.data.events = arr;
    }

    saveEvent(): void {
        this.selected = null;
    }

    add(): void {
        if (!this.data.events) {
            this.data.events = new Array();
        }
        this.selected = { date: null, year: null, century: null, name: null };
        this.data.events.push(this.selected);
    }


}