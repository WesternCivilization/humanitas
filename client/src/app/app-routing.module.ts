import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';

import { TemplatesModule } from '../templates/templates.module';
import { ControlsModule } from '../controls/controls.module';

import {
  SocialLoginModule,
  AuthServiceConfig,
  GoogleLoginProvider,
  FacebookLoginProvider,
  LinkedinLoginProvider
} from 'ng4-social-login';
import { LoginService } from './login.service';


const CONFIG = new AuthServiceConfig([
  {
    id: GoogleLoginProvider.PROVIDER_ID,
    provider: new GoogleLoginProvider('instituto-leibniz')
  },
  {
    id: FacebookLoginProvider.PROVIDER_ID,
    provider: new FacebookLoginProvider('345873245862912')
  },
  {
    id: LinkedinLoginProvider.PROVIDER_ID,
    provider: new LinkedinLoginProvider('77htw850n55sn9')
  }
]);



export function provideConfig() {
  return CONFIG;
}


@NgModule({
  imports: [
    HttpModule,
    ControlsModule,
    TemplatesModule,
    RouterModule.forRoot([
      { path: '', redirectTo: '/activities', pathMatch: 'full' },
      { path: 'login', loadChildren: 'login/login.module#LoginModule' },
      { path: 'activities', loadChildren: 'activities/activities.module#ActivitiesModule' },
      { path: 'explorer', loadChildren: 'explorer/explorer.module#ExplorerModule' },
      { path: 'libraries', loadChildren: 'libraries/libraries.module#LibrariesModule' },
      { path: 'users', loadChildren: 'users/users.module#UsersModule' },
      { path: 'users/:id', loadChildren: 'user-detail/user-detail.module#UserDetailModule' },
      { path: 'terminal', loadChildren: 'terminal/terminal.module#TerminalModule' },
      { path: 'edit-fragment/:id', loadChildren: 'edit-fragment/edit-fragment.module#EditFragmentModule' },
      { path: 'edit-tag/:id', loadChildren: 'edit-tag/edit-tag.module#EditTagModule' }
    ]),
    SocialLoginModule
  ],
  exports: [RouterModule],
  providers: [{
    provide: AuthServiceConfig,
    useFactory: provideConfig
  }, LoginService]
})
export class AppRoutingModule { }
