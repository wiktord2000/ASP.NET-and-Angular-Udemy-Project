import { MembersService } from 'src/app/_services/members.service';
import { take } from 'rxjs';
import { AccountService } from './../../_services/account.service';
import { environment } from './../../../environments/environment.prod';
import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { faTrash, faUpload, faBan } from '@fortawesome/free-solid-svg-icons'
import { FileUploader } from 'ng2-file-upload';
import { User } from 'src/app/_models/user';
import { Photo } from 'src/app/_models/photo';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() member: Member;
  user: User;
  uploader: FileUploader;
  faTrash = faTrash;
  faUpload = faUpload;
  faBan = faBan;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;
 

  constructor(private accountService : AccountService, private memberService : MembersService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any){
    this.hasBaseDropzoneOver = e;
  }

  setMainPhoto(photo: Photo){
    this.memberService.setMainPhoto(photo.id).subscribe(() => {
        this.user.photoUrl = photo.url;
        this.accountService.setCurrentUser(this.user);
        this.member.photoUrl = photo.url;
        this.member.photos.forEach(p => {
          if(p.isMain) p.isMain = false;
          if(p.id === photo.id) p.isMain = true;
        })
      }
    )
  }

  deletePhoto(photoId: number){
    this.memberService.deletePhoto(photoId).subscribe(() => {
      this.member.photos = this.member.photos.filter(p => p.id !== photoId);
    })
  }

  initializeUploader(){
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user.token,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    })

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if(response){
        const photo = JSON.parse(response);
        this.member.photos.push(photo);
      }
    }
  }

}
