import { Component, OnInit } from '@angular/core';
import {AppService} from '../../services/app.service';
import {Search} from '../../models/search.model';
import {ActivatedRoute, Router} from '@angular/router';

@Component({
    selector: 'app-search',
    templateUrl: './search.component.html',
    styleUrls: ['./search.component.css'],
    providers: [AppService]
})
export class SearchComponent implements OnInit {
    world='';
	searchResult : Search[] = [];
	length : number = 0;
	isLoadingResults = false;

    constructor( private appService: AppService, private router: Router, private route: ActivatedRoute ) { }

    ngOnInit() { }

    onSearch() {
    	this.isLoadingResults = true;
		this.appService.SearchApply(this.world)
			.subscribe(data=>{
			    console.log('SearchApply');
			    console.log(data);
				this.searchResult = data;
				this.length = this.searchResult.length;
				this.isLoadingResults = false;
			},(err) => {
				this.isLoadingResults = false;
				console.log(err);
			});
    }
}
