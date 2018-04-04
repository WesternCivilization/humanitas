import { Component, Input } from '@angular/core';
import { TopicaService } from '../providers/topica.service';
import { AppSettings } from '../core/app.settings';

@Component({
    templateUrl: './library-panel.component.html',
    selector: "library-panel"
})
export class LibraryPanelComponent {

    @Input()
    public data: any;

    constructor(private _topicaService: TopicaService,
        public _settings: AppSettings) {
    }

    save(data: any): void {
        this._topicaService.save(data).subscribe(result => this._topicaService.notifyAll(this.data));
    }

    selectBook(libraryId: string): void {
        this._topicaService.libraryBook(this.data.tagId, libraryId).subscribe(book => {
            this.data.libraryBook = book;
        })
}

    setAuthor(name: string): void {
        if (this.data) {
            this.data.author = name;
            if (this.data.title) {
                this.data.name = this.data.title + ' [' + this.data.author + ']';
            }
        }
    }

    setTitle(name: string): void {
        if (this.data) {
            this.data.title = name;
            if (this.data.author) {
                this.data.name = this.data.title + ' [' + this.data.author + ']';
            }
        }
    }

    setPurchasedDate(value: string): void {
        debugger;
        let parts = value.split('|');
        this.data.libraryBook.purchasedDate = parts[0];
    }


}