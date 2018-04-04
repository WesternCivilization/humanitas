import { Injectable } from '@angular/core';
import { forEach } from '@angular/router/src/utils/collection';
import { debuglog } from 'util';
import { IOption } from 'ng-select';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';

@Injectable()
export class Breadcrumb {

    static path: Array<any> = [];
    static filterInvoker: Subject<Array<IOption>> = new Subject<Array<IOption>>();
    static filter: Observable<Array<IOption>> = Breadcrumb.filterInvoker.asObservable();

    public getPath(): Array<any> {
        return Breadcrumb.path;
    }

    public clear(): void {
        Breadcrumb.path = [];
    }

    public append(label: string, icon: string, link: string): void {
        let url = link;
        let path = this.trimPath(link);
        let queryParams = {};
        if (link.indexOf(';') != -1) {
            let pars = link.split(';')[1].split('&');
            pars.forEach(element2 => {
                let keyValue = element2.split('=');
                queryParams[keyValue[0]] = keyValue[1];
            });
            link = link.split(';')[0];
        }
        if (path.length == 0 || (path.length > 0 && path[path.length - 1].label != label)) {
            path.push({ label: label, icon: icon, link: link, queryParams: queryParams, url: url });
        }
        Breadcrumb.path = path;
    }

    public trimPath(url: string): Array<any> {
        let arr = new Array();
        let doNotAdd = false;
        Breadcrumb.path.forEach(element => {
            if (element.url.toUpperCase() == url.toUpperCase()) {
                doNotAdd = true;
            }
            else {
                if (!doNotAdd) {
                    arr.push(element);
                }
            }
        });
        return arr;
    }

    public pathStartWith(expression: string): boolean {
        if (Breadcrumb.path.length > 0) {
            return Breadcrumb.path[0].link.startsWith(expression);
        }
        else {
            return false;
        }
    }

    applyFilters(options: Array<IOption>): void {
        Breadcrumb.filterInvoker.next(options);
    }

}