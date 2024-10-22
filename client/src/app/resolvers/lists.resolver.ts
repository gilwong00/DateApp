import { Injectable } from '@angular/core';
import { User } from '../models/user';
import { Resolve, Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { AlertifyService } from '../services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ListsResolver implements Resolve<User[]> {
	pageNumber:number = 1;
	pageSize:number = 5;
	likesParam = 'Likers';

	constructor(
		private userService: UserService,
		private router: Router,
		private alertify: AlertifyService
	) { }

	resolve(): Observable<User[]> {
		return this.userService.getUsers(this.pageNumber, this.pageSize, null, this.likesParam).pipe(
			catchError(err => {
				this.alertify.error('Problem retrieving data');
				this.router.navigate(['/home']);
				return of(null); // return null observable
			})
		);
	}
}
