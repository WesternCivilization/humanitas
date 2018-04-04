import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TopicaService } from './topica.service';
import { ZeteticaService } from './zetetica.service';
import { TerminalService } from './terminal.service';
import { UserService } from './user.service';

@NgModule({
    declarations: [ ],
    exports: [ ],
    imports: [
        CommonModule
    ],
    providers: [ TopicaService, ZeteticaService, TerminalService, UserService ]
})
export class ProvidersModule { }
