import { Pipe, PipeTransform } from '@angular/core';
import { datepickerAnimation } from 'ngx-bootstrap/datepicker/datepicker-animations';
import { utcToLocalTimeFormat } from '../_enums/utcToLocalTimeFormat';

@Pipe({
  name: 'utcToLocalTime'
})
export class UtcToLocalTimePipe implements PipeTransform {

  transform(utcDate: string,
    format: utcToLocalTimeFormat | string): string {

    var browserLanguage = navigator.language;

    if (format === utcToLocalTimeFormat.SHORT) {
      let date = new Date(utcDate).toLocaleDateString(browserLanguage);
      let time = new Date(utcDate).toLocaleTimeString(browserLanguage);

      return `${date}, ${time}`
    }
    else if (format === utcToLocalTimeFormat.SHORT_DATE) {
      return new Date(utcDate).toLocaleDateString(browserLanguage);
    } 
    else if(format === utcToLocalTimeFormat.SHORT_TIME) {
      return new Date(utcDate).toLocaleTimeString(browserLanguage);
    }
    else if (format === utcToLocalTimeFormat.FULL) {
      return new Date(utcDate).toString();
    }

    return new Date(utcDate).toString();
    
  }

}
