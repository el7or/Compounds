import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-account-creation',
  templateUrl: './account-creation.component.html',
  styleUrls: ['./account-creation.component.scss']
})
export class AccountCreationComponent implements OnInit {
  visibility: boolean = false;
  loading: boolean = false;
  constructor() { }

  ngOnInit(): void {
  }

}
