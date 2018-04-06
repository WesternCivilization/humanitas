import { HashLocationStrategy, PathLocationStrategy, Location, LocationStrategy } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';

import { ServiceWorkerModule } from '@angular/service-worker';
import { AppComponent } from './app.component';

import { environment } from '../environments/environment';

import { TemplatesModule } from '../templates/templates.module';

import { AppSettings } from '../core/app.settings';
import { Breadcrumb } from '../core/breadcrumb';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    TemplatesModule,
    ServiceWorkerModule.register('/ngsw-worker.js', { enabled: environment.production })
  ],
  providers: [
    { provide: LocationStrategy, useClass: HashLocationStrategy },
    AppSettings,
    Breadcrumb
  ],
    bootstrap: [AppComponent]
})
export class AppModule { }
