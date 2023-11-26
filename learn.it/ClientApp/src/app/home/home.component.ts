import { Component } from '@angular/core';
import { ActivityDto } from '../models/lastactivity.dto';

@Component({
  selector: 'app-home',
  styleUrls: ['./home.component.css'],
  templateUrl: './home.component.html',
})
export class HomeComponent {


  usersLastActivity: ActivityDto[] = [
    {
      id: 1,
      name: "Nazwa zestawu 1",
      avatarUrl: "/assets/plus-icon.svg",
      username: "Userasdasd1",
      isPrivate: true
    },
    {
      id: 2,
      name: "Nazwa zestawu 2",
      avatarUrl: "/assets/plus-icon.svg",
      username: "User2",
      isPrivate: false
    },
    {
      id: 3,
      name: "Nazwa zestawu 2",
      avatarUrl: "/assets/plus-icon.svg",
      username: "User1",
      isPrivate: true
    }
  ]




}
