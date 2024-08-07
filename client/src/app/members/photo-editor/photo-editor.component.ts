import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from 'src/app/_models/Photo';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.scss']
})
export class PhotoEditorComponent implements OnInit {

  @Input() member:Member | undefined;
  uploader: FileUploader | undefined;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;
  user: User | undefined;

  constructor(private account:AccountService,private memberservice:MembersService){
    this.account.currentUser$.subscribe({
      next:user=>{
        if(user) this.user=user;
      }
    })

  }


  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropzoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    }
    this.uploader.onSuccessItem=(item, response, status, headers) =>
    {
        if(response){
          const Photo=JSON.parse(response);
          this.member?.photos.push(Photo);
        }
    }
  }

  setMainPhoto(photo:Photo)
  {
    this.memberservice.setMainPhoto(photo.id).subscribe({
      next:()=>{
        if(this.user && this.member)
        {
          this.user.photoUrl=photo.url;
          this.account.setCurrentUser(this.user);
          this.member.photoUrl=photo.url;
          this.member.photos.forEach(p=>{
            if(p.isMain) p.isMain=false;
            if(p.id===photo.id) p.isMain=true;
          })

        }
      }
    })

  }

  deletePhoto(photoId: number) {
    this.memberservice.deletePhoto(photoId).subscribe({
      next: _ => {
        if (this.member) {
          this.member.photos = this.member?.photos.filter(x => x.id !== photoId)
        }
      }
    })
  }
 

}
