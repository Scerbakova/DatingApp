import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.scss'],
})
export class DatePickerComponent implements ControlValueAccessor {
  @Input() label = ''
  @Input() maxDate?: Date
  bsConfig?: Partial<BsDatepickerConfig>
  
  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
    this.bsConfig = {
      containerClass: "theme-red",
      dateInputFormat: "DD MMMM YYYY"
    }
  }

  writeValue() {}
  registerOnChange() {}
  registerOnTouched() {}

  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }
}
