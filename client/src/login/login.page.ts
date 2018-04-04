import { Component, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import {
    AuthService,
    FacebookLoginProvider,
    GoogleLoginProvider,
    LinkedinLoginProvider
} from 'ng4-social-login';

@Component({
    templateUrl: './login.page.html'
})
export class LoginPage implements OnInit {

    constructor(private authService: AuthService, private route: Router) {
    }

    ngOnInit(): void {
        this.authService.authState.subscribe((user) => {
            if (user) {
                this.route.navigate(['/activities']);
            }
        });
    }

    signInWithGoogle(): void {
        this.authService.signIn(GoogleLoginProvider.PROVIDER_ID);
    }

    signInWithFB(): void {
        this.authService.signIn(FacebookLoginProvider.PROVIDER_ID);
    }

    signInWithLinkedIN(): void {
        this.authService.signIn(LinkedinLoginProvider.PROVIDER_ID);
    }


}