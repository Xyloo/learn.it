import { Component } from '@angular/core';
import { UserGroupDto } from '../../../models/user-groups.dto';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.css']
})
export class GroupsComponent {


  leaveGroup(id: number) {

  }







  userGroups: UserGroupDto[] = [
    {
      id: 1,
      name: 'giga grupa2'
    },
    {
      id: 2,
      name: 'giga grupa'
    },
    {
      id: 3,
      name: 'grupa'
    }
  ]

}
