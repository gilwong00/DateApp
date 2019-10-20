import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/models/message';
import { UserService } from 'src/app/services/user.service';
import { AuthService } from 'src/app/services/auth.service';
import { AlertifyService } from 'src/app/services/alertify.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-message',
  templateUrl: './member-message.component.html',
  styleUrls: ['./member-message.component.css'],
})
export class MemberMessageComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.loadMessage();
  }

  loadMessage() {
    // using a + before a string converts a string to a number if possible
    const userId: number = +this.authService.decodedToken.nameid;
    this.userService
      .getMessageThread(userId, this.recipientId)
      .pipe(
        tap((messages: Message[]) => {
          messages.forEach((message: Message) => {
            if (!message.isRead && message.recipientId === userId) {
              this.userService.markMessageIsRead(userId, message.id);
            }
          });
        })
      )
      .subscribe(
        messages => {
          this.messages = messages;
        },
        err => this.alertify.error(err)
      );
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.userService
      .sendMessage(this.authService.decodedToken.nameid, this.newMessage)
      .subscribe(
        (message: Message) => {
          this.messages.unshift(message);
          this.newMessage.content = '';
        },
        err => this.alertify.error(err)
      );
  }
}
