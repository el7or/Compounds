import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialogModule } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRippleModule } from '@angular/material/core';
import { LanguageService } from '../services/language.serives';
import { HttpClientModule, HttpClientJsonpModule } from '@angular/common/http';
import { MatTreeModule } from '@angular/material/tree';
import { MatSelectModule } from '@angular/material/select';
import { MatDividerModule } from '@angular/material/divider';
import { MatPaginatorModule } from '@angular/material/paginator';
import { StylePaginatorDirective } from './style-paginator.directive';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatChipsModule } from '@angular/material/chips';
import { SliceWordsPipe } from './pipes/slice-words.pipe';
import { MatSortModule } from '@angular/material/sort';
import { EditorModule } from "@tinymce/tinymce-angular";
import {ScrollingModule} from '@angular/cdk/scrolling';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';
import {MatRadioModule} from '@angular/material/radio';
import {MatButtonToggleModule} from '@angular/material/button-toggle';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@NgModule({
  declarations: [StylePaginatorDirective, SliceWordsPipe],
  imports: [
    MatNativeDateModule,
    MatDatepickerModule,
    HttpClientModule,
    HttpClientJsonpModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatPaginatorModule,
    MatGridListModule,
    MatDividerModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatSnackBarModule,
    MatStepperModule,
    MatTooltipModule,
    MatMenuModule,
    MatDialogModule,
    MatTableModule,
    MatCheckboxModule,
    MatRippleModule,
    MatTreeModule,
    MatSelectModule,
    MatChipsModule,
    EditorModule,
    ScrollingModule,
    TimepickerModule.forRoot()
  ],
  exports: [
    MatNativeDateModule,
    MatDatepickerModule,
    StylePaginatorDirective,
    MatPaginatorModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    MatGridListModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatSnackBarModule,
    MatStepperModule,
    MatTooltipModule,
    MatMenuModule,
    MatDialogModule,
    MatTableModule,
    MatCheckboxModule,
    MatRippleModule,
    HttpClientModule,
    HttpClientJsonpModule,
    MatTreeModule,
    MatSelectModule,
    MatDividerModule,
    MatChipsModule,
    MatSortModule,
    MatAutocompleteModule,
    SliceWordsPipe,
    EditorModule,
    ScrollingModule,
    TimepickerModule,
    MatRadioModule,
    MatButtonToggleModule,
    MatProgressBarModule
  ],
  providers: [LanguageService]
})
export class SharedModule {
  constructor(private languageService: LanguageService) {
    // this.languageService.selectLang(this.languageService.lang);
  }
}
