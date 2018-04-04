import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { AppCommonModule } from '../common/common.module';
import { TemplatesModule } from '../templates/templates.module';
import { ControlsModule } from '../controls/controls.module';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { NgModule } from '@angular/core';

import { EditTagPage } from './edit-tag.page';

import { AreaPanelComponent } from './area-panel.component';
import { AuthorPanelComponent } from './author-panel.component';
import { BookPanelComponent } from './book-panel.component';
import { InstitutionPanelComponent } from './institution-panel.component';
import { LawPanelComponent } from './law-panel.component';
import { LibraryPanelComponent } from './library-panel.component';
import { PeriodPanelComponent } from './period-panel.component';
import { SkillPanelComponent } from './skill-panel.component';
import { StatePanelComponent } from './state-panel.component';
import { TagPanelComponent } from './tag-panel.component';
import { TopicPanelComponent } from './topic-panel.component';
import { ReferencesPanelComponent } from './references-panel.component';
import { ImagePanelComponent } from './image-panel.component';
import { EventsPanelComponent } from './events-panel.component';
import { LinksPanelComponent } from './links-panel.component';

@NgModule({
    declarations: [
        EditTagPage,
        AreaPanelComponent,
        AuthorPanelComponent,
        BookPanelComponent,
        InstitutionPanelComponent,
        LawPanelComponent,
        LibraryPanelComponent,
        PeriodPanelComponent,
        SkillPanelComponent,
        StatePanelComponent,
        TagPanelComponent,
        TopicPanelComponent,
        ReferencesPanelComponent,
        ImagePanelComponent,
        EventsPanelComponent,
        LinksPanelComponent
    ],
    imports: [
        AppCommonModule,
        CommonModule,
        TemplatesModule,
        ControlsModule,
        InfiniteScrollModule,
        RouterModule.forChild([
            { path: '', component: EditTagPage }
        ]),
    ],
    providers: [  ]
})
export class EditTagModule { }
