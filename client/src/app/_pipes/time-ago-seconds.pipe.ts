import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeAgoSeconds'
})
export class TimeAgoSecondsPipe implements PipeTransform {

  transform(value: string): string {
    const split = value.split(' ', 2);

    if (+split[0] <= 59 && split[1].includes('second') ) {
      return 'Less than a minute ago';
    } else {
      return value;
    }

    return value;
  }

}
