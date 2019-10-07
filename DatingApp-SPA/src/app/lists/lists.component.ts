import { Component, OnInit } from '@angular/core';
import { User } from '../models/user';
import { Pagination, PaginatedResult } from '../models/pagination';
import { AuthService } from '../services/auth.service';
import { UserService } from '../services/user.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../services/alertify.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css'],
})
export class ListsComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  likesParam: string;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private route: ActivatedRoute,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });

    this.likesParam = 'Likers';
  }

  loadUsers() {
    this.userService
      .getUsers(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        null,
        this.likesParam
      )
      .subscribe(
        (res: PaginatedResult<User[]>) => {
          this.users = res.results;
          this.pagination = res.pagination;
        },
        err => {
          this.alertify.error(err);
        }
      );
  }

  changePage(event: any) {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }
}
