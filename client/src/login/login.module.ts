import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { AppCommonModule } from '../common/common.module';
import { CoreModule } from '../core/core.module';
import { ProvidersModule } from '../providers/providers.module';
import { TemplatesModule } from '../templates/templates.module';
import { IntelliSenseModule } from '../intellisense/intellisense.module';
import { NgModule } from '@angular/core';

import { LoginPage } from './login.page';
import { Breadcrumb } from '../core/breadcrumb';


@NgModule({
    declarations: [
        LoginPage
    ],
    imports: [
        AppCommonModule,
        CommonModule,
        ProvidersModule,
        InfiniteScrollModule,
        CoreModule,
        RouterModule.forChild([
            { path: '', component: LoginPage }
        ]),
    ],
    providers: [ Breadcrumb ]
})
export class LoginModule { }
