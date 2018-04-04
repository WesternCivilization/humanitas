import { Component, Input } from '@angular/core';
import { TopicaService } from '../providers/topica.service';

@Component({
    templateUrl: './book-panel.component.html',
    selector: "book-panel"
})
export class BookPanelComponent {

    @Input()
    public data: any;

    constructor(private _topicaService: TopicaService) {
    }

    selectBook(libraryId: string): void {

    }

    save(data: any): void {
        this._topicaService.save(data).subscribe(result => this._topicaService.notifyAll(this.data));
    }

}