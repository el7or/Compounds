import { Component, Input, OnInit } from '@angular/core';
import { LanguageService } from 'src/app/services/language.serives';

@Component({
  selector: 'app-form-layout',
  templateUrl: './form-layout.component.html',
  styleUrls: ['./form-layout.component.scss']
})
export class FormLayoutComponent implements OnInit {
  @Input() image: string = '';

  constructor(){
   }

  ngOnInit(): void {
  }

}
