import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
	baseUrl: string = environment.apiUrl + 'auth/';
	jwtHelper = new JwtHelperService();
	decodedToken: any;
	currentUser: User;
	photoUrl = new BehaviorSubject<string>('../../assets/uesr.png');
	currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) {}

  login(model: any) {
    return this.http
      .post(this.baseUrl + 'login', model, { withCredentials: true }) // gets cookie
      .pipe(
        map((response: any) => {
          const user = response;
          if (user) {
						localStorage.setItem('token', user.token);
						localStorage.setItem('user', JSON.stringify(user.userInfo));
						this.decodedToken = this.jwtHelper.decodeToken(user.token);
						this.currentUser = user.userInfo;
						this.changeMemberPhoto(this.currentUser.photoUrl);
          }
        })
      );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
	}
	
	isLoggedIn() {
		const token = localStorage.getItem('token');
		return !this.jwtHelper.isTokenExpired(token);
	}

	changeMemberPhoto(photoUrl: string) {
		this.photoUrl.next(photoUrl);
	}
}
