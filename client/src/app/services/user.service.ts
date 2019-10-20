import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user';
import { PaginatedResult } from '../models/pagination';
import { map } from 'rxjs/operators';
import { Message } from '../models/message';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  baseUrl: string = environment.apiUrl;
  constructor(private http: HttpClient) {}

  getUsers(
    page?,
    itemsPerPage?,
    userParams?,
    likesParam?
  ): Observable<PaginatedResult<User[]>> {
    const paginatedResults: PaginatedResult<User[]> = new PaginatedResult<
      User[]
    >();
    let params = new HttpParams();

    if (page && itemsPerPage) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParams) {
      for (let key in userParams) {
        params = params.append(key, userParams[key]);
      }
    }

    if (likesParam === 'Likers') {
      params = params.append('likers', 'true');
    }

    if (likesParam === 'Likees') {
      params = params.append('likees', 'true');
    }

    return this.http
      .get<User[]>(this.baseUrl + 'users', { observe: 'response', params })
      .pipe(
        map(response => {
          paginatedResults.results = response.body;

          if (response.headers.get('Pagination')) {
            paginatedResults.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
          }
          return paginatedResults;
        })
      );
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto(userId: number, photoId: number) {
    return this.http.post(
      this.baseUrl + 'users/' + userId + '/photos/' + photoId + '/setMain',
      {}
    );
  }

  deletePhoto(userId: number, photoId: number) {
    return this.http.delete(
      this.baseUrl + 'users/' + userId + '/photos/' + photoId
    );
  }

  sendLike(id: number, recipientId: number) {
    return this.http.post(
      this.baseUrl + 'users/' + id + '/like/' + recipientId,
      {}
    );
  }

  getMessages(
    userId: number,
    page?,
    itemsPerPage?,
    messageContainer?: string
  ) {
    const paginatedResults: PaginatedResult<Message[]> = new PaginatedResult<
      Message[]
    >();

    let params = new HttpParams();
    params = params.append('MessageContainer', messageContainer);

    if (page && itemsPerPage) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this.http
      .get<Message[]>(this.baseUrl + 'users/' + userId + '/messages', {
        observe: 'response',
        params,
      })
      .pipe(
        map(response => {
          paginatedResults.results = response.body;

          if (response.headers.get('Pagination')) {
            paginatedResults.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
          }
          return paginatedResults;
        })
      );
	}
	
	getMessageThread(id: number, recipientId: number) {
		this.baseUrl + '/users/' + id + '/messages/thread/' + recipientId
		return this.http.get<Message[]>(`${this.baseUrl}users/${id}/messages/thread/${recipientId}`);
	}

	sendMessage(id: number, message: Message) {
		return this.http.post(`${this.baseUrl}users/${id}/messages`, message);
	}

	deleteMessage(messageId: number, userId: number) {
		return this.http.post(`${this.baseUrl}users/${userId}/messages/${messageId}`, {});
	}

	markMessageIsRead(userId: number, messageId: number) {
		return this.http.post(`${this.baseUrl}users/${userId}/messages/${messageId}/read`, {})
			.subscribe();
	}
}
