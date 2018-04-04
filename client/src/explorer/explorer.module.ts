import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { AppCommonModule } from '../common/common.module';
import { TemplatesModule } from '../templates/templates.module';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { NgModule } from '@angular/core';

import { ExplorerPage } from './explorer.page';


@NgModule({
    declarations: [
        ExplorerPage
    ],
    imports: [
        AppCommonModule,
        CommonModule,
        TemplatesModule,
        InfiniteScrollModule,
        RouterModule.forChild([
            { path: '', component: ExplorerPage }
        ]),
    ],
    providers: [  ]
})
export class ExplorerModule { }
