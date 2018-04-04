import { Component, Input } from '@angular/core';

@Component({
    templateUrl: './breadcrumb.template.html',
    selector: "breadcrumb-template"
})
export class BreadcrumbTemplate {

    @Input()
    public data:Array<any>;

}