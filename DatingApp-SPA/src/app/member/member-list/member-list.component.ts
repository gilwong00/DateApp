import { Component, OnInit } from '@angular/core';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { AlertifyService } from '../../services/alertify.service';

@Component({
	selector: 'app-member-list',
	templateUrl: './member-list.component.html',
	styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
	users: User[];
	constructor(
		private userService: UserService,
		private alterify: AlertifyService
	) { }

	ngOnInit() {
		this.loadUsers();
	}

	loadUsers() {
		this.userService.getUsers().subscribe(
			(users: User[]) => {
				this.users = users;
			},
			err => {
				this.alterify.error(err);
			}
		);
	}
}
