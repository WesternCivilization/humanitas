import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CoreModule } from '../core/core.module';

import { ActivitiesTemplate } from './activities.template';
import { BreadcrumbTemplate } from './breadcrumb.template';
import { TagTemplate } from './tag.template';
import { PlayerTemplate } from './player.template';
import { ScoreTemplate } from './score.template';
import { QuickSearch } from './quick-search';
import { PreloadTemplate } from './preload.template';

import { Player } from '../core/player';


@NgModule({
    declarations: [
        ActivitiesTemplate,
        BreadcrumbTemplate,
        TagTemplate,
        PlayerTemplate,
        ScoreTemplate,
        QuickSearch,
        PreloadTemplate
    ],
    imports: [
        CommonModule,
        RouterModule,
        CoreModule
    ],
    exports: [
        ActivitiesTemplate,
        BreadcrumbTemplate,
        TagTemplate,
        PlayerTemplate,
        ScoreTemplate,
        QuickSearch,
        PreloadTemplate
    ],
    providers: [ Player ]
})
export class TemplatesModule { }
