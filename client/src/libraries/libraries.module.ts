import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { AppCommonModule } from '../common/common.module';
import { CoreModule } from '../core/core.module';
import { ProvidersModule } from '../providers/providers.module';
import { TemplatesModule } from '../templates/templates.module';
import { NgModule } from '@angular/core';

import { LibrariesPage } from './libraries.page';
import { Breadcrumb } from '../core/breadcrumb';


@NgModule({
    declarations: [
        LibrariesPage
    ],
    imports: [
        AppCommonModule,
        CommonModule,
        ProvidersModule,
        InfiniteScrollModule,
        TemplatesModule,
        CoreModule,
        RouterModule.forChild([
            { path: '', component: LibrariesPage }
        ]),
    ],
    providers: [ Breadcrumb ]
})
export class LibrariesModule { }
