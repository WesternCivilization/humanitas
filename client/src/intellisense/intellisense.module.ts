import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { CoreModule } from '../core/core.module';
import { ProvidersModule } from '../providers/providers.module';

import { BatchData } from './batchdata';
import { TopicReorg } from './topicreorg';


@NgModule({
    declarations: [
        BatchData,
        TopicReorg
    ],
    imports: [
        CommonModule,
        RouterModule,
        ProvidersModule,
        CoreModule
    ],
    exports: [
        BatchData,
        TopicReorg
    ],
    providers: [ ]
})
export class IntelliSenseModule { }
