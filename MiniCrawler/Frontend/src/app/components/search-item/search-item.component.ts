import { Component, OnInit, Input,ViewEncapsulation } from '@angular/core';
import {Search} from '../../models/search.model';

@Component({
    selector: 'app-search-item',
    templateUrl: './search-item.component.html',
    styleUrls: ['./search-item.component.css'],
    encapsulation: ViewEncapsulation.Emulated
})
export class SearchItemComponent implements OnInit {

    @Input() search;
    element: Search;

    ngOnInit() {
        this.element = this.search;
    }

}

