import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { Breadcrumb } from './breadcrumb';
import { AppSettings } from './app.settings';
import { Player } from './player';

@NgModule({
    declarations: [ ],
    exports: [ ],
    imports: [
        CommonModule
    ],
    providers: [ Breadcrumb, AppSettings, Player ]
})
export class CoreModule { }
