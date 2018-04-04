import { Component, ViewChild, OnInit, Output, Input, EventEmitter } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { AppSettings } from '../core/app.settings';

@Component({
    templateUrl: './file.component.html',
    selector: "file-control"
})
export class FileComponent implements OnInit {

    @Input("label")
    public label: string;

    @Input("icon")
    public icon: string;

    @Input("urlpost")
    public urlpost: string;

    @Output("uploading")
    public uploading: EventEmitter<string> = new EventEmitter();

    @Output("uploaded")
    public uploaded: EventEmitter<string> = new EventEmitter();

    files: any;

    constructor(private _http: Http,
        private _settings: AppSettings) {
    }

    ngOnInit(): void { }

    selectFile(event) {
        this.files = event.srcElement.files;
        if (this.files && this.files.length > 0) {
            this.uploadFile();
        }
    }

    uploadFile(): void {
        this.uploading.emit(this.files[0].name);
        let data = new FormData();
        data.append('uploadFile', this.files[0], this.files[0].name);
        let headers = new Headers()
        //headers.append('Content-Type', 'json');  
        //headers.append('Accept', 'application/json');  
        let options = new RequestOptions({ headers: headers });
        this._http.post(this._settings.ApiUrl + this.urlpost + "?token=" + AppSettings.UserToken,
            data, options).subscribe(result => {
                if(result)
                {
                    this.uploaded.emit(this.files[0].name);
                    this.files = new Array();
                }
            });
    }


}