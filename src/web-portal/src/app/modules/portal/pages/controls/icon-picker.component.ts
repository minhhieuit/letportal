import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { PageRenderedControl, DefaultControlOptions } from 'app/core/models/page.model';
import * as _ from 'lodash';
import { ExtendedControlValidator } from 'app/core/models/extended.models';
import { StaticResources } from 'portal/resources/static-resources';

@Component({
    selector: 'let-icon-picker',
    templateUrl: './icon-picker.component.html'
})
export class IconPickerComponent implements OnInit {
    iconFilterOptions: Observable<string[]>;
    _icons = StaticResources.iconsList()

    @Input()
    form: FormGroup

    @Input()
    formControlKey: string    

    @Input()
    tooltip: string

    @Input()
    control: PageRenderedControl<DefaultControlOptions>

    @Input()
    validators: Array<ExtendedControlValidator> = []

    constructor() { }

    ngOnInit(): void { 
        this.iconFilterOptions = this.form.get(this.formControlKey).valueChanges.pipe(
            startWith(''),
            map(value => this._filterIcon(value))
        )
    }

    private _filterIcon(choosingIconValue: string): Array<string> {
        const filterValue = choosingIconValue.toLowerCase()

        return this._icons.filter(op => op.toLowerCase().includes(filterValue))
    }

    isInvalid(validatorName: string): boolean {
        return this.form.get(this.control.name).hasError(validatorName)
    }

    getErrorMessage(validatorName: string) {
        return _.find(this.validators, validator => validator.validatorName === validatorName).validatorErrorMessage
    }
}
