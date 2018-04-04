import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { AppCommonModule } from '../common/common.module';
import { ProvidersModule } from '../providers/providers.module';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { NgModule } from '@angular/core';

import { UserDetailPage } from './user-detail.page';


@NgModule({
    declarations: [
        UserDetailPage
    ],
    imports: [
        AppCommonModule,
        CommonModule,
        ProvidersModule,
        InfiniteScrollModule,
        RouterModule.forChild([
            { path: '', component: UserDetailPage }
        ]),
    ],
    providers: [  ]
})
export class UserDetailModule { }
