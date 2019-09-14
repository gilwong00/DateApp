import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/app/models/user';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/services/user.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
	selector: 'app-member-edit',
	templateUrl: './member-edit.component.html',
	styleUrls: ['./member-edit.component.css'],
})
export class MemberEditComponent implements OnInit {
	user: User;
	@ViewChild('editForm', { static: true }) editForm: NgForm;
	@HostListener('window:beforeunload', ['$event'])
	unloadNotification($event: any) {
		if (this.editForm.dirty) {
			$event.returnValue = true;
		}
	}

	constructor(
		private rotue: ActivatedRoute,
		private alertify: AlertifyService,
		private userServicve: UserService,
		private authService: AuthService
	) { }

	ngOnInit() {
		this.rotue.data.subscribe(data => {
			this.user = data['user'];
		});
	}

	updateUser() {
		this.userServicve.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next => {
			this.alertify.success('Profile updated successfully');
			this.editForm.reset(this.user);
		}, err => this.alertify.error(err));
	}
}
