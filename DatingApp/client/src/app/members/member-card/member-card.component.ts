import { Member} from 'src/app/_models/member';
import { Component, Input, OnInit } from '@angular/core';
import { faUser, faEnvelope, faHeart } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  @Input() member: Member;
  faUser = faUser;
  faEnvelope = faEnvelope;
  faHeart = faHeart;

  constructor() { }

  ngOnInit(): void {
  }

}
