import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'sliceWords',
  pure: false
})
export class SliceWordsPipe implements PipeTransform {

  transform(value: string, wordsCount: number): string {
    if (value.split(/\s+/).length > wordsCount) {
      return value.split(/\s+/).slice(0, wordsCount).join(" ").concat(" ...");
    }
    else { return value }
  }

}
