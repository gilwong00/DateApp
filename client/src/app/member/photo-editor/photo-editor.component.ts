import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from 'src/app/models/photo';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/services/auth.service';
import { UserService } from 'src/app/services/user.service';
import { AlertifyService } from 'src/app/services/alertify.service';

@Component({
	selector: 'app-photo-editor',
	templateUrl: './photo-editor.component.html',
	styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
	@Input() photos: Photo[];
	//@Output() updateMainPhoto = new EventEmitter<string>();
	uploader: FileUploader;
	hasBaseDropZoneOver: boolean = false;
	baseUrl = environment.apiUrl;
	currentMainPhoto: Photo;

	constructor(
		private authService: AuthService,
		private userService: UserService,
		private alertify: AlertifyService
	) { }

	ngOnInit() {
		this.initializeUploader();
	}

	fileOverBase(e: any): void {
		this.hasBaseDropZoneOver = e;
	}

	initializeUploader() {
		this.uploader = new FileUploader({
			url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
			authToken: 'Bearer ' + localStorage.getItem('token'),
			isHTML5: true,
			allowedFileType: ['image'],
			removeAfterUpload: true,
			autoUpload: false,
			maxFileSize: 10 * 1024 * 1024 // 10mb
		});

		this.uploader.onAfterAddingFile = file => file.withCredentials = false;

		this.uploader.onSuccessItem = (item, response, status, headers) => {
			if (response) {
				const res: Photo = JSON.parse(response);
				const photo = {
					id: res.id,
					url: res.url,
					dateAdded: res.dateAdded,
					description: res.description,
					isMain: res.isMain
				};

				this.photos.push(photo);

				if (photo.isMain) {
					this.updatePhoto(photo);
				}
			}
		}
	}

	setMainPhoto(photo: Photo) {
		this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(next => {
			this.currentMainPhoto = this.photos.filter(p => p.isMain).pop();
			this.currentMainPhoto.isMain = false;
			photo.isMain = true;
			this.updatePhoto(photo);
		}, err => this.alertify.error(err))
	}

	deletePhoto(photoId: number) {
		this.alertify.confirm('Are you sure you want to delete this photo?', () => {
			this.userService.deletePhoto(this.authService.decodedToken.nameid, photoId).subscribe(() => {
				this.photos.splice(this.photos.findIndex(p => p.id === photoId), 1);
				this.alertify.success('Photo has been deleted');
			}, err => this.alertify.error('Failed to delete the photo'));
		})
	}

	updatePhoto(photo: Photo) {
		this.authService.changeMemberPhoto(photo.url);
		this.authService.currentUser.photoUrl = photo.url;
		localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
	}
}
