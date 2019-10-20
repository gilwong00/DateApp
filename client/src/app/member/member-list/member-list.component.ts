import { Component, OnInit } from '@angular/core';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { AlertifyService } from '../../services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from '../../models/pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  users: User[];
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' },
  ];
  userParams: any = {};
  pagination: Pagination;

  constructor(
    private userService: UserService,
    private alterify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      console.log();
      this.users = data['users'].results;
      this.pagination = data['users'].pagination;
		});
		
		// default search params
		this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
		this.userParams.minAge = 18;
		this.userParams.maxAge = 99;
		this.userParams.orderBy = 'lastActive';
  }

  changePage(event: any) {
    this.pagination.currentPage = event.page;
    this.loadUsers();
	}
	
	resetFilters() {
		this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
		this.userParams.minAge = 18;
		this.userParams.maxAge = 99;
		this.loadUsers();
	}

  loadUsers() {
    this.userService
      .getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
      .subscribe(
        (res: PaginatedResult<User[]>) => {
          this.users = res.results;
          this.pagination = res.pagination;
        },
        err => {
          this.alterify.error(err);
        }
      );
  }
}
