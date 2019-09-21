import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';
import { Router } from '@angular/router';

@Component({
	selector: 'app-nav',
	templateUrl: './nav.component.html',
	styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
	// user login model
	model: any = {};
	photoUrl: string;

	constructor(
		public authService: AuthService,
		private alertify: AlertifyService,
		private router: Router
	) { }

	ngOnInit() { 
		this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
	}

	login() {
		this.authService.login(this.model).subscribe(
			next => {
				this.alertify.success('Logging in successfully');
			},
			err => {
				this.alertify.error(err);
			}, () => {
				this.router.navigate(['/members']);
			}
		);
	}

	loggedIn() {
		return this.authService.isLoggedIn();
	}

	logout() {
		localStorage.removeItem('token');
		localStorage.removeItem('user');
		this.authService.decodedToken = null;
		this.authService.currentUser = null;
		this.alertify.message('Logged out');
		this.router.navigate(['/home']);
	}
}
