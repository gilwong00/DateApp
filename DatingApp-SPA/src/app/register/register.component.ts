import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Component({
	selector: 'app-register',
	templateUrl: './register.component.html',
	styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
	@Input() values: any;
	@Output() cancelRegister = new EventEmitter();

	model: any = {};

	constructor(private authService: AuthService) { }

	ngOnInit() {
		console.log('val', this.values)
	}

	register() {
		this.authService.register(this.model).subscribe(() => {
			console.log('success')
		}, err => console.error(err))
	}

	cancel() {
		this.cancelRegister.emit(false);
	}
}
