import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms'
import { AppComponent } from './app.component';
import { ValuesComponent } from './values/values.component';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './services/auth.service';

@NgModule({
	declarations: [
		AppComponent,
		ValuesComponent,
		NavComponent
	],
	imports: [
		BrowserModule,
		HttpClientModule,
		FormsModule	
	],
	providers: [
		AuthService
	],
	bootstrap: [AppComponent]
})
export class AppModule { }
