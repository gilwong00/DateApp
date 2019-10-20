import { Component, OnInit } from '@angular/core';
import { Message } from '../models/message';
import { Pagination, PaginatedResult } from '../models/pagination';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../services/alertify.service';

@Component({
	selector: 'app-messages',
	templateUrl: './messages.component.html',
	styleUrls: ['./messages.component.css'],
})
export class MessagesComponent implements OnInit {
	messages: Message[];
	pagination: Pagination;
	messageContainer = 'Unread';

	constructor(
		private userService: UserService,
		private authService: AuthService,
		private route: ActivatedRoute,
		private alertify: AlertifyService
	) { }

	ngOnInit() {
		this.route.data.subscribe(data => {
			this.messages = data['messages'].result;
			this.pagination = data['messages'].pagination;
		});
	}

	loadMessages() {
		this.userService
			.getMessages(
				this.authService.decodedToken.nameid,
				this.pagination.currentPage,
				this.pagination.itemsPerPage,
				this.messageContainer
			)
			.subscribe(
				(res: PaginatedResult<Message[]>) => {
					this.messages = res.results;
					this.pagination = res.pagination;
				},
				err => this.alertify.error(err)
			);
	}

	pagedChanged(event: any): void {
		this.pagination.currentPage = event.page;
		this.loadMessages();
	}

	deleteMessage(messageId: number) {
		this.alertify.confirm(
			'Are you sure you want to delete this message?',
			() => {
				this.userService
					.deleteMessage(messageId, this.authService.decodedToken.nameid)
					.subscribe(() => {
						this.messages = this.messages.filter(
							message => message.id !== messageId
						);
						this.alertify.success('Message has been deleted');
					}, err => this.alertify.error('Failed to delete message'));
			}
		);
	}
}
