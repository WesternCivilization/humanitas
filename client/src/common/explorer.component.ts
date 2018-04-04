import { Component, ViewChild, Output, Input, EventEmitter } from '@angular/core';

import { TopicaService } from '../providers/topica.service';
import { Subscription } from 'rxjs/Subscription';
import { AppSettings } from '../core/app.settings';
import { ActivatedRoute } from '@angular/router';
import { Breadcrumb } from '../core/breadcrumb';
import { OnInit } from '@angular/core/src/metadata/lifecycle_hooks';
import { ZeteticaService } from '../providers/zetetica.service';
import { TimelineActivity } from '../providers/timelineactivity';

@Component({
    templateUrl: './explorer.component.html',
    selector: "explorer"
})
export class ExplorerComponent{
}