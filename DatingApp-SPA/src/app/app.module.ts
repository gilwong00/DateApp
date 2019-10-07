import { BrowserModule, HammerGestureConfig, HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { RouterModule } from '@angular/router';
import { BsDropdownModule, TabsModule, BsDatepickerModule, PaginationModule, ButtonsModule } from 'ngx-bootstrap';
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
import { PhotoEditorComponent } from './member/photo-editor/photo-editor.component';
import { FileUploadModule } from 'ng2-file-upload';
import { TimeAgoPipe } from 'time-ago-pipe';
import { ListsResolver } from './resolvers/lists.resolver';


export function tokenGetter() {
	return localStorage.getItem('token');
}

export class CustomHammerConfig extends HammerGestureConfig {
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
		MemberEditComponent,
		PhotoEditorComponent,
		TimeAgoPipe
	],
	imports: [
		BrowserModule,
		BrowserAnimationsModule,
		HttpClientModule,
		FormsModule,
		ReactiveFormsModule,
		PaginationModule.forRoot(),
		TabsModule.forRoot(),
		ButtonsModule.forRoot(),
		BsDropdownModule.forRoot(),
		BsDatepickerModule.forRoot(),
		RouterModule.forRoot(Router),
		NgxGalleryModule,
		JwtModule.forRoot({
			config: {
				tokenGetter,
				whitelistedDomains: ['localhost:5000'],
				blacklistedRoutes: ['localhost:5000/api/auth']
			}
		}),
		FileUploadModule
	],
	providers: [
		AuthService,
		ErrorInterceptorProvider,
		MemberDetailResolver,
		MemberListResolver,
		MemberEditResolver,
		{ provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig },
		PreventUnsavedChanges,
		ListsResolver
	],
	bootstrap: [AppComponent]
})
export class AppModule { }
