import { Component, forwardRef, Input, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { LanguageService } from 'src/app/services/language.serives';
import { WebApiService } from 'src/app/services/webApi.service';

@Component({
  selector: 'app-upload-file',
  templateUrl: './upload-file.component.html',
  styleUrls: ['./upload-file.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: forwardRef(() => UploadFileComponent),
    }
  ]
})
export class UploadFileComponent implements ControlValueAccessor, OnInit {
  @Input() label: string = '';
  @Input() hint: string = '';
  @Input() error: string | null = null;
  @Input() isRequired: boolean = false;
  @Input() isMultiple: boolean = false;
  @Input() acceptedExtensions: string[] = ["png", "gif", "jpg"];
  @Input() AttachmentTypeID: any;
  @Input() veiwOnly: boolean = false;
  @Input() maxFileSize: number = 0.5 * 1024 * 1024; //bytes; // 500kB max
  acceptedExtensionsString: string = '';
  isDisabled: boolean = false;
  imagesArray: Iimage[] = [];
  isFileUploaded: boolean = false;
  inValidSize: boolean = false;
  constructor(public languageService: LanguageService, private webApiService: WebApiService) { }
  ngOnInit(): void {
    this.acceptedExtensionsString = this.acceptedExtensions.reduce((str, ext) => `.${ext},${str}`, '');
  }
  uploadFiles(files: FileList | any) {
    for (var j = 0; j < files.length; j++) {
      const file: File = files[j];
      if (file.size < this.maxFileSize) {
        this.inValidSize = false
        let reader = new FileReader();
        reader.onload = (thisFile: ProgressEvent) => {
          let currentTarget: FileReader = thisFile.currentTarget as FileReader;
          let binaryString = currentTarget.result as string;

          if (!this.isMultiple) this.imagesArray = [];

          this.imagesArray.push({
            sizeInBytes: file.size,
            fileName: file.name,
            fileBase64: btoa(binaryString)
          });
          //files loop end
          if (j === files.length) {
            if (this.isMultiple) {
              this.onChange(this.imagesArray);
            } else {
              this.onChange(this.imagesArray[0]);
            }
          }
        };
        reader.readAsBinaryString(file);
      } else if (file.size > this.maxFileSize) {
        this.inValidSize = true;
      }
    }
  }
  deleteImage(index: number) {
    console.log(22)
    this.imagesArray.splice(index, 1);
    if (this.isMultiple) {
      this.onChange(this.imagesArray);
    } else {
      this.onChange(this.imagesArray[0]);
    }
  }
  writeValue(obj: any): void {
    // console.log(obj)
    if (obj) {
      if (this.isMultiple) {
        if (obj.length) {
          obj.forEach((image: Iimage) => {
            this.imagesArray.push({
              path: image.path
            })
          });
        }
      } else {
        // obj.replace('http','https')
        // this.webApiService.getImage(obj).subscribe(res=>{
        //     console.log(res);

        // })
        // fetch(obj,{headers:{}}).then(res => {
        //   res.blob().then(bas => {
        //     console.log(bas);
        //   })
        // })
        this.imagesArray.push({
          path: obj && obj.path ? obj.path :obj
        });
      }
    }
  }
  onChange = (data: any) => this.writeValue(data);

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  onTouched = () => { };
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    this.isDisabled = isDisabled
  }
}
interface Iimage {
  sizeInBytes?: number;
  fileName?: string;
  fileBase64?: string;
  path?: string;
}
