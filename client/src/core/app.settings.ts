import { Injectable } from '@angular/core';

function _window(): any {
    // return the global native browser window object
    return window;
}


@Injectable()
export class AppSettings {

    public static FilterTag: string = null;
    public static FilterLibraryTag: string = null;
    public static SortType: string = "4";

    constructor() {
        if (_window().location.port == 4200) {
            this.ApiUrl = "http://" + _window().location.hostname + ":57978/";
        }
        else {
            this.ApiUrl = "http://rafaelmelo.web1612.kinghost.net/humanitasapi";
        }
    }

    public ApiUrl: string;
    public static UserToken: string;
    public static UserTypeId: number;

    public format(text: string): string {
        if (text == null) return text;
        return text.replace(/\n/g, '<br />');
    }

    public getUserType(): number {
        return AppSettings.UserTypeId;
    }

    public cleanText(text: string): string {
        if (text == null) return text;
        return text.replace(/'/g, "\\'");
    }

    public getEntityType(category: string, type: number): string {
        if (category == "tag") {
            switch (type) {
                case 0: return "area";
                case 1: return "period";
                case 2: return "author";
                case 3: return "institution";
                case 4: return "book";
                case 7: return "topic";
                case 8: return "law";
                case 9: return "state";
                case 10: return "skill";
                case 12: return "library";
            }
        }
        else if (category == "fragment") {
            switch (type) {
                case 0: return "quote";
                case 1: return "note";
                case 2: return "question";
                case 3: return "video";
                case 4: return "article";
                case 5: return "audio";
            }
        }
        return "";
    }

    public getEntityIcon(type: string): string {
        if (type == "domain-9") return "fa fa-cubes";
        else if (type == "domain-10") return "fa fa-music";
        else if (type == "domain-11") return "fa fa-calculator";
        else if (type == "domain-12") return "fa fa-map";
        else if (type == "domain-13") return "fa fa-money";
        else if (type == "domain-14") return "fa fa-gears";
        else if (type == "domain-15") return "fa fa-venus-mars";
        else if (type == "domain-16") return "fa fa-bullhorn";
        else if (type == "domain-17") return "fa fa-gavel";
        else if (type == "domain-18") return "fa fa-globe";
        else if (type == "domain-19") return "fa fa-child";
        else if (type == "domain-20") return "fa fa-eye";
        else if (type == "domain-41") return "fa fa-circle";
        else if (type == "quote") return "fa fa-quote-left";
        else if (type == "note") return "fa fa-comment";
        else if (type == "question") return "fa fa-question";
        else if (type == "video") return "fa fa-film";
        else if (type == "article") return "fa fa-file-text-o";
        else if (type == "audio") return "glyphicon glyphicon-volume-up";
        else if (type == "area") return "fa fa-arrows";
        else if (type == "period") return "fa fa-clock-o";
        else if (type == "author") return "fa fa-male";
        else if (type == "institution") return "fa fa-university";
        else if (type == "book") return "fa fa-book";
        else if (type == "topic") return "fa fa-tag";
        else if (type == "law") return "fa fa-balance-scale";
        else if (type == "state") return "fa fa-shield";
        else if (type == "skill") return "fa fa-code-fork";
        else if (type == "library") return "fa fa-inbox";
        else {
            return "fa fa-circle";
        }
    }

    getDomainName(id: any): any {
        if (id == 9) return "Filosofia dos Princípios";
        else if (id == 10) return "Filosofia da Música";
        else if (id == 11) return "Filosofia da Matemática";
        else if (id == 12) return "Filosofia das Nações";
        else if (id == 13) return "Filosofia da Economia";
        else if (id == 14) return "Filosofia da Tecnologia";
        else if (id == 15) return "Filosofia dos Costumes";
        else if (id == 16) return "Filosofia da Cultura";
        else if (id == 17) return "Filosofia do Estado";
        else if (id == 18) return "Filosofia da Alma";
        else if (id == 19) return "Filosofia da Consciência";
        else if (id == 20) return "Filosofia do Entendimento";
        else return "Não Classificado";
    }

    public getViewLink(type: string, domainId: number, id: string, label: string): any {
        return ['/explorer', { domainId: 18, parent: id, parentName: label, parentType: type }];
    }

    public getEditLink(type: string, id: string): string {
        if (type == "domain-9") return "/";
        else if (type == "domain-10") return "/";
        else if (type == "domain-11") return "/";
        else if (type == "domain-12") return "/";
        else if (type == "domain-13") return "/";
        else if (type == "domain-14") return "/";
        else if (type == "domain-15") return "/";
        else if (type == "domain-16") return "/";
        else if (type == "domain-17") return "/";
        else if (type == "domain-18") return "/";
        else if (type == "domain-19") return "/";
        else if (type == "domain-20") return "/";
        else if (type == "domain-41") return "/";
        else if (type == "quote") return "/edit-fragment/" + id;
        else if (type == "note") return "/edit-fragment/" + id;
        else if (type == "question") return "/edit-fragment/" + id;
        else if (type == "video") return "/edit-fragment/" + id;
        else if (type == "article") return "/edit-fragment/" + id;
        else if (type == "audio") return "/edit-fragment/" + id;
        else if (type == "area") return "/edit-tag/" + id;
        else if (type == "period") return "/edit-tag/" + id;
        else if (type == "author") return "/edit-tag/" + id;
        else if (type == "institution") return "/edit-tag/" + id;
        else if (type == "book") return "/edit-tag/" + id;
        else if (type == "topic") return "/edit-tag/" + id;
        else if (type == "law") return "/edit-tag/" + id;
        else if (type == "state") return "/edit-tag/" + id;
        else if (type == "skill") return "/edit-tag/" + id;
        else if (type == "library") return "/edit-tag/" + id;
        else return "fa-circle";
    }

    public trunc(text: string, length: number): string {
        if (text && text.length > length)
            return text.substring(0, length) + "..";
        else
            return text;
    }


}