import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { SelectModule } from 'ng-select';

import { CoreModule } from '../core/core.module';
import { ProvidersModule } from '../providers/providers.module';
import { TemplatesModule } from '../templates/templates.module';

import { TagSelectComponent } from './tagselect.component';
import { ExplorerComponent } from './explorer.component';
import { TreeViewComponent } from './treeview.component';

@NgModule({
    declarations: [
        TagSelectComponent,
        ExplorerComponent,
        TreeViewComponent
    ],
    exports: [
        TagSelectComponent,
        ExplorerComponent,
        TreeViewComponent
    ],
    imports: [
        CommonModule,
        SelectModule,
        RouterModule,
        InfiniteScrollModule,
        CoreModule,
        TemplatesModule,
        ProvidersModule
    ],
    providers: [ ]
})
export class AppCommonModule { }
