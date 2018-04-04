import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { CoreModule } from '../core/core.module';
import { ProvidersModule } from '../providers/providers.module';

import { DateComponent } from './date.component';
import { FloatComponent } from './float.component';
import { PreloadComponent } from './preload.component';
import { TextComponent } from './text.component';
import { FileComponent } from './file.component';
import { TextAreaComponent } from './textarea.component';
import { PreloadSelect } from './preload.select';

@NgModule({
    declarations: [
        DateComponent,
        FloatComponent,
        PreloadComponent,
        TextComponent,
        FileComponent,
        TextAreaComponent,
        PreloadSelect
    ],
    exports: [
        DateComponent,
        FloatComponent,
        PreloadComponent,
        TextComponent,
        FileComponent,
        TextAreaComponent,
        PreloadSelect
    ],
    imports: [
        CommonModule,
        CoreModule,
        FormsModule,
        ProvidersModule
    ],
    providers: []
})
export class ControlsModule { }
