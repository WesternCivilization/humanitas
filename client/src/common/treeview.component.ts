import { Component, ViewChild, OnInit, Output, Input, EventEmitter } from '@angular/core';

import { TopicaService } from '../providers/topica.service';
import { Subscription } from 'rxjs/Subscription';
import { AppSettings } from '../core/app.settings';

@Component({
    templateUrl: './treeview.component.html',
    selector: "treeview"
})
export class TreeViewComponent implements OnInit {

    @Input()
    public folders: Array<any> = [];

    @Input()
    public domainId:number;    

    constructor(public _settings:AppSettings, 
        private _topicaService:TopicaService) {
    }

    ngOnInit(): void { }

    loadChildren(node:any):void {
        if(node.nodes)
        {
            node.nodes = null;
        }
        else{
            node.nodes = new Array();
            this._topicaService.folders(this.domainId, node.value).subscribe(nodes => { node.nodes = nodes; });
        }
    }


}