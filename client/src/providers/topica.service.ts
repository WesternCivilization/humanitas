import { Injectable, OnInit, NgZone, EventEmitter } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import { AppSettings } from '../core/app.settings';
import { Observable } from 'rxjs/Observable';
import { IOption } from 'ng-select';

@Injectable()
export class TopicaService implements OnInit {

    private cacheDb: any;
    private static once: boolean = false;
    private static cacheDbReady: boolean = false;
    public static saved: EventEmitter<any> = new EventEmitter();

    constructor(private _http: Http,
        public _settings: AppSettings,
        private zone: NgZone) {
        this.ngOnInit();
    }

    ngOnInit(): void {

        function try_read(obj) {
            let self = obj;

            function db_check_error(obj) {
                let self = obj;
                return function (_, error) {
                    self.createDatabase();
                };
            }

            return function (transaction) {
                transaction.executeSql("SELECT COUNT(*) AS total FROM AutoComplete",
                    [], function (_, result) {
                        console.info("cache already installed. rows: " + result.rows[0].total);
                        TopicaService.cacheDbReady = true;
                    },
                    db_check_error(self));
            }
        }

        try {
            if (!(<any>window).openDatabase) {
                console.log('Web SQL not supported.');
            } else {
                var shortName = 'diariofilosofico';
                var version = '1.0';
                var displayName = 'My Important Database';
                var maxSize = 65536; // in bytes
                this.cacheDb = (<any>window).openDatabase(shortName, version, displayName, maxSize);
                this.cacheDb.transaction(try_read(this));
            }
        } catch (e) {
            // Error handling code goes here.
            if (e == 2) {
                // Version number mismatch.
                alert("Invalid database version.");
            } else {
                alert("Unknown error " + e + ".");
            }
        }


    }

    createDatabase(): void {
        this._http
            .get(this._settings.ApiUrl + '/api/sqllite/autocomplete')
            .map((response: any) => {
                return response;
            }).subscribe(response => {
                if (!TopicaService.once) {
                    TopicaService.once = true;
                    this.installDb(response.text());
                }
            });
    }

    installDb(scripts: string): void {
        var obj = this;
        this.cacheDb.transaction(function (transaction) {
            transaction.executeSql("SELECT COUNT(*) AS total FROM AutoComplete",
                [], function (trans, result) {
                    console.info("cache already installed. rows: " + result.rows[0].total);
                    TopicaService.cacheDbReady = true;
                }, function (trans, error) {
                    console.info("installing cache...");
                    let cmds = scripts.split('`');
                    function error_handler(sql) {
                        var statement = sql;
                        return function (_, error) {
                            console.error(error);
                            console.warn(statement);
                        }
                    };
                    for (let i = 0; i < cmds.length; i++) {
                        let sql = cmds[i];
                        if (sql[0] == '"') {
                            sql = sql.substring(1);
                        }
                        if (sql.length > 2) {
                            trans.executeSql(sql, [],
                                function (_, result) { },
                                error_handler(sql));
                        }
                    }
                    console.info("cache installation is completed.");
                    TopicaService.cacheDbReady = true;
                });
        });
    }

    execute(scripts: string): void {
        var obj = this;
        this.cacheDb.transaction(function (transaction) {
            console.info("running scripts...");
            let cmds = scripts.split('`');
            function error_handler(sql) {
                var statement = sql;
                return function (_, error) {
                    console.error(error);
                    console.warn(statement);
                }
            };
            for (let i = 0; i < cmds.length; i++) {
                let sql = cmds[i];
                if (sql[0] == '"') {
                    sql = sql.substring(1);
                }
                if (sql.length > 2) {
                    transaction.executeSql(sql, [],
                        function (_, result) { },
                        error_handler(sql));
                }
            }
            console.info("execution is completed.");
        });
    }

    save(data: any): Observable<any> {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this._http
            .post(this._settings.ApiUrl + '/api/topica/save?token=' + AppSettings.UserToken, data, options)
            .map((response: any) => { this.execute(response.json().sql); return response.json(); });
    }

    run(data: any): Observable<any> {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this._http
            .post(this._settings.ApiUrl + '/api/terminal/run?token=' + AppSettings.UserToken, data, options)
            .map((response: any) => { this.execute(response.json().sql); return response.json(); });
    }

    notifyAll(data: any): void {
        TopicaService.saved.emit(data);
    }

    autocomplete(type: string, expression: any): Observable<Array<IOption>> {
        if (TopicaService.cacheDbReady) {
            return new Observable((observer) => {
                function observer_function(observer) {
                    function result_function(observer) {
                        var observer = observer;
                        return function (_, result) {
                            let count = result.rows.length;
                            var arr = new Array();
                            for (let i = 0; i < count; i++) {
                                arr.push(result.rows[i]);
                            }
                            observer.next(arr);
                            observer.complete();
                            observer = null;
                        }
                    }
                    return function (transaction) {
                        let where = "1 = 1";
                        if (type == "all" || type == "tag") {
                            where = "Category = 'TAG'";
                        }
                        else if (type == "author") {
                            where = "Category = 'TAG' AND Type = 2";
                        }
                        else if (type == "topic") {
                            where = "Category = 'TAG' AND Type = 7";
                        }
                        else if (type == "book") {
                            where = "Category = 'TAG' AND Type = 4";
                        }
                        else if (type == "area") {
                            where = "Category = 'TAG' AND Type = 0";
                        }
                        else if (type == "period") {
                            where = "Category = 'TAG' AND Type = 1";
                        }
                        else if (type == "law") {
                            where = "Category = 'TAG' AND Type = 8";
                        }
                        else if (type == "state") {
                            where = "Category = 'TAG' AND Type = 9";
                        }
                        else if (type == "skill") {
                            where = "Category = 'TAG' AND Type = 10";
                        }
                        else if (type == "institution") {
                            where = "Category = 'TAG' AND Type = 3";
                        }
                        else if (type == "library") {
                            where = "Category = 'TAG' AND Type = 12";
                        }
                        else if (type == "domain") {
                            where = "Category = 'DOMAIN'";
                        }
                        else if (type == "fragmenttype") {
                            where = "Category = 'FRAGMENTTYPE'";
                        }
                        else if (type == "tagtype") {
                            where = "Category = 'TAGTYPE'";
                        }
                        else if (type == "sorttype") {
                            where = "Category = 'SORTTYPE'";
                        }
                        else {
                            alert("wrong type:" + type);
                        }
                        let sql = "SELECT Id AS value, Name AS label, Type as type FROM AutoComplete WHERE " + where + " AND Name LIKE '%" + expression + "%' ORDER BY CASE WHEN Name LIKE '" + expression + "%' THEN 1 WHEN Name LIKE '%" + expression + "%' THEN 2 ELSE 3 END, Name LIMIT 10";
                        transaction.executeSql(sql, [], result_function(observer), function (_, error) {
                            console.error(error);
                        });

                    }
                }
                this.cacheDb.transaction(observer_function(observer));
            });
        }
        else {
            let url = this._settings.ApiUrl + '/api/topica/autocomplete?type=' + type + '&exp=' + expression + '&token=' + AppSettings.UserToken;
            return this._http
                .get(url)
                .map((response: any) => response.json());
        }
    }

    select(type: string, value: any): Observable<IOption> {
        if (TopicaService.cacheDbReady) {
            return new Observable((observer) => {
                function observer_function(observer) {
                    function result_function(observer) {
                        var observer = observer;
                        return function (_, result) {
                            if (result.rows.length > 0) {
                                observer.next({ value: String(result.rows[0].value), label: String(result.rows[0].label) });
                                observer.complete();
                                observer = null;
                            }
                            else {
                                observer.next(null);
                                observer.complete();
                                observer = null;
                            }
                        }
                    }
                    return function (transaction) {
                        let where = "1 = 1";
                        if (type == "all" || type == "tag") {
                            where = "Category = 'TAG'";
                        }
                        else if (type == "author") {
                            where = "Category = 'TAG' AND Type = 2";
                        }
                        else if (type == "topic") {
                            where = "Category = 'TAG' AND Type = 7";
                        }
                        else if (type == "book") {
                            where = "Category = 'TAG' AND Type = 4";
                        }
                        else if (type == "area") {
                            where = "Category = 'TAG' AND Type = 0";
                        }
                        else if (type == "period") {
                            where = "Category = 'TAG' AND Type = 1";
                        }
                        else if (type == "law") {
                            where = "Category = 'TAG' AND Type = 8";
                        }
                        else if (type == "state") {
                            where = "Category = 'TAG' AND Type = 9";
                        }
                        else if (type == "skill") {
                            where = "Category = 'TAG' AND Type = 10";
                        }
                        else if (type == "institution") {
                            where = "Category = 'TAG' AND Type = 3";
                        }
                        else if (type == "library") {
                            where = "Category = 'TAG' AND Type = 12";
                        }
                        else if (type == "domain") {
                            where = "Category = 'DOMAIN'";
                        }
                        else if (type == "fragmenttype") {
                            where = "Category = 'FRAGMENTTYPE'";
                        }
                        else if (type == "tagtype") {
                            where = "Category = 'TAGTYPE'";
                        }
                        else if (type == "sorttype") {
                            where = "Category = 'SORTTYPE'";
                        }
                        else {
                            alert("wrong type:" + type);
                        }
                        let sql = "SELECT Id AS value, Name AS label, Type as type FROM AutoComplete WHERE " + where + " AND Id = '" + value + "'";
                        transaction.executeSql(sql, [], result_function(observer), function (_, error) {
                            console.error(error);
                        });

                    }
                }
                this.cacheDb.transaction(observer_function(observer));
            });
        }
        else {
            let url = this._settings.ApiUrl + '/api/topica/select?type=' + type + '&value=' + value + '&token=' + AppSettings.UserToken;
            return this._http
                .get(url)
                .map((response: any) => response.json());
        }
    }

    folders(domainId: number, parent: string): Observable<Array<IOption>> {
        let url = this._settings.ApiUrl + '/api/topica/folders?domainId=' + domainId + '&parent=' + parent + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

    references(parent: string): Observable<Array<any>> {
        let url = this._settings.ApiUrl + '/api/topica/references?parent=' + parent + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

    libraries(): Observable<Array<any>> {
        let url = this._settings.ApiUrl + '/api/topica/libraries?token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

    books(libraryId: string, tags: Array<String>, start: number): Observable<any> {
        let url = this._settings.ApiUrl + '/api/topica/books?libraryId=' + libraryId + '&tags=' + tags.join(",") + '&start=' + start + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

    detail(id: string): any {
        let url = this._settings.ApiUrl + '/api/topica/detail?tagId=' + id + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

    libraryBook(bookId: string, libraryId: string): any {
        let url = this._settings.ApiUrl + '/api/topica/book-detail?bookId=' + bookId + '&libraryId=' + libraryId + '&token=' + AppSettings.UserToken;
        return this._http
            .get(url)
            .map((response: any) => response.json());
    }

}

