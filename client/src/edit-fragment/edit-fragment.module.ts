import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { AppCommonModule } from '../common/common.module';
import { TemplatesModule } from '../templates/templates.module';
import { ControlsModule } from '../controls/controls.module';
import { NgModule } from '@angular/core';

import { EditFragmentPage } from './edit-fragment.page';

import { FragmentPanelComponent } from './fragment-panel.component';
import { ImagePanelComponent } from './image-panel.component';
import { ReferencesPanelComponent } from './references-panel.component';

@NgModule({
    declarations: [
        EditFragmentPage,
        FragmentPanelComponent,
        ImagePanelComponent,
        ReferencesPanelComponent
    ],
    imports: [
        AppCommonModule,
        CommonModule,
        TemplatesModule,
        ControlsModule,
        RouterModule.forChild([
            { path: '', component: EditFragmentPage }
        ]),
    ],
    providers: [  ]
})
export class EditFragmentModule { }
