import { Component } from '@angular/core';
import { UserSetsDto } from '../models/user-sets.dto';
import { Location } from '@angular/common';

@Component({
  selector: 'app-user-sets',
  templateUrl: './user-sets.component.html',
  styleUrls: ['./user-sets.component.css']
})
export class UserSetsComponent {

  constructor(private location: Location) { }
  userSets: UserSetsDto[] = [
    {
      id: 1,
      setName: "Fizyka - wzory",
      description: '/assets/temp-avatar.png',
      username: "User1"
    },
    {
      id: 1,
      setName: "Fizyka - wzory",
      description: '/assets/temp-avatar.png',
      username: "User1"
    },
    {
      id: 1,
      setName: "Fizyka - wzory",
      description: '/assets/temp-avatar.png',
      username: "User1"
    },
    {
      id: 1,
      setName: "Fizyka - wzory",
      description: '/assets/temp-avatar.png',
      username: "User1"
    },
    {
      id: 1,
      setName: "Fizyka - wzory",
      description: '/assets/temp-avatar.png',
      username: "User1"
    },
    {
      id: 1,
      setName: "Fizyka - wzory",
      description: '/assets/temp-avatar.png',
      username: "User1"
    },
    {
      id: 1,
      setName: "Fizyka - wzory",
      description: '/assets/temp-avatar.png',
      username: "User1"
    },
    {
      id: 1,
      setName: "Fizyka - wzory",
      description: '/assets/temp-avatar.png',
      username: "User1"
    }
  ]

/*  deleteItem(itemId: number) {
    this.yourService.deleteItem(itemId).subscribe({
      next: (response) => {
        this.items = this.items.filter(item => item.id !== itemId);
      },
      error: (error) => {
        console.error('Error deleting item:', error);
      }
    });
  }*/


  goBack() {
    this.location.back(); 
  }

}
