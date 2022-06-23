import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-reply-layout',
  templateUrl: './reply-layout.component.html',
  styleUrls: ['./reply-layout.component.scss']
})
export class ReplyLayoutComponent implements OnInit {
  @Input() image: string = '';
  constructor() { }

  ngOnInit(): void {
  }

}
