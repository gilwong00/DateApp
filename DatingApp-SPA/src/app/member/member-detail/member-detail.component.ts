import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/user';
import { UserService } from 'src/app/services/user.service';
import { AlertifyService } from 'src/app/services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryImage, NgxGalleryOptions, NgxGalleryAnimation } from 'ngx-gallery';

@Component({
	selector: 'app-member-detail',
	templateUrl: './member-detail.component.html',
	styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit {
	user: User;
	galleryOptions: NgxGalleryOptions[];
	galleryImages: NgxGalleryImage[];

	constructor(
		private userService: UserService,
		private alertify: AlertifyService,
		// get params from routes
		private route: ActivatedRoute
	) { }

	ngOnInit() {
		// this gets the data from the resolver instead
		this.route.data.subscribe(data => {
			this.user = data['user']
		});

		this.galleryOptions = [
			{
				width: '500px',
				height: '500px',
				imagePercent: 100,
				thumbnailsColumns: 4,
				imageAnimation: NgxGalleryAnimation.Slide,
				preview: false
			}
		];

		this.galleryImages = this.getImages();
	}

	getImages() {
		const imageUrls = [];
		for (const photo of this.user.photos) {
			imageUrls.push({
				small: photo.url,
				medium: photo.url,
				big: photo.url
			});
		}

		return this.user.photos.map(photo => ({
			small: photo.url,
			medium: photo.url,
			big: photo.url
		}));
	}

	// loadUser() {
	//   /*
	// 		id will come as a string, if we add a + in front of the string, 
	// 		it converts it to a number
	// 	*/
	// 	this.userService.getUser(+this.route.snapshot.params['id']).subscribe(
	// 		(user: User) => {
	// 			this.user = user;
	// 		},
	// 		err => {
	// 			this.alertify.error(err);
	// 		}
	// 	);
	// }
}
