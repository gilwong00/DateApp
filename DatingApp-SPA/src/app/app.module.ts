import { BrowserModule, HammerGestureConfig, HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms'
import { RouterModule } from '@angular/router';
import { BsDropdownModule, TabsModule } from 'ngx-bootstrap';
import { JwtModule } from '@auth0/angular-jwt';
import { NgxGalleryModule } from 'ngx-gallery';

import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './services/auth.service';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvider } from './services/error.interceptor';
import { MemberListComponent } from './member/member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { Router } from './routes';
import { MemberCardComponent } from './member/member-card/member-card.component';
import { MemberDetailComponent } from './member/member-detail/member-detail.component';
import { MemberDetailResolver } from './resolvers/member-detail.resolver';
import { MemberListResolver } from './resolvers/member-list.resolver';
import { MemberEditComponent } from './member/member-edit/member-edit.component';
import { MemberEditResolver } from './resolvers/member-edit.resolver ';
import { PreventUnsavedChanges } from './guards/prevent-unsavedChanges.guard';

export function tokenGetter() {
	return localStorage.getItem('token');
}

export class CustomHammerConfig extends HammerGestureConfig  {
	overrides = {
			pinch: { enable: false },
			rotate: { enable: false }
	};
}

@NgModule({
	declarations: [
		AppComponent,
		NavComponent,
		HomeComponent,
		RegisterComponent,
		MemberListComponent,
		ListsComponent,
		MessagesComponent,
		MemberCardComponent,
		MemberDetailComponent,
		MemberEditComponent
	],
	imports: [
		BrowserModule,
		HttpClientModule,
		FormsModule,
		TabsModule.forRoot(),
		BsDropdownModule.forRoot(),
		RouterModule.forRoot(Router),
		NgxGalleryModule,
		JwtModule.forRoot({
			config: {
				tokenGetter,
				whitelistedDomains: ['localhost:5000'],
				blacklistedRoutes: ['localhost:5000/api/auth']
			}
		})
	],
	providers: [
		AuthService,
		ErrorInterceptorProvider,
		MemberDetailResolver,
		MemberListResolver,
		MemberEditResolver,
		{ provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig },
		PreventUnsavedChanges
	],
	bootstrap: [AppComponent]
})
export class AppModule { }
