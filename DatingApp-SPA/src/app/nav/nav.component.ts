import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';

@Component({
	selector: 'app-nav',
	templateUrl: './nav.component.html',
	styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
	// user login model
	model: any = {};
	constructor(
		public authService: AuthService,
		private alertify: AlertifyService
	) { }

	ngOnInit() { }

	login() {
		this.authService.login(this.model).subscribe(
			next => {
				this.alertify.success('Logging in successfully')
			},
			err => {
				this.alertify.error(err);
			}
		);
	}

	loggedIn() {
		return this.authService.isLoggedIn();
	}

	logout() {
		localStorage.removeItem('token');
		this.alertify.message('Logged out');
	}
}
