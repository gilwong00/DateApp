import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';

@Component({
	selector: 'app-register',
	templateUrl: './register.component.html',
	styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
	@Input() values: any;
	@Output() cancelRegister = new EventEmitter();

	model: any = {};

	constructor(
		private authService: AuthService,
		private alertify: AlertifyService
	) { }

	ngOnInit() { }

	register() {
		this.authService
			.register(this.model)
			.subscribe(
				() => this.alertify.success('Registration successful'),
				err => this.alertify.error(err)
			);
	}

	cancel() {
		this.cancelRegister.emit(false);
	}
}
