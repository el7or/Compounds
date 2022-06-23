import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-account-activation',
  templateUrl: './account-activation.component.html',
  styleUrls: ['./account-activation.component.scss']
})
export class AccountActivationComponent implements OnInit {
  visibility: boolean = false;
  loading: boolean = false;
  constructor() { }

  ngOnInit(): void {
  }

}
