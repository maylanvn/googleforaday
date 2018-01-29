import { Component, OnInit } from '@angular/core';
import {AppService} from '../../services/app.service';
import {ActivatedRoute, Router} from '@angular/router';
import {Index} from '../../models/index.model';
import { Observable } from 'rxjs/Rx';
import {error} from "util";

@Component({
    selector: 'app-index',
    templateUrl: './index.component.html',
    styleUrls: ['./index.component.css'],
    providers: [ AppService]
})
export class IndexComponent implements OnInit {
    url:'';
    indexResult = {
		PagesIndexed : 0,
		KeywordsIndexed : 0
    };
    isLoadingResults = false;

    constructor(private appService: AppService, private router: Router, private route: ActivatedRoute) { }

    ngOnInit() { }

    onIndex() {
        this.isLoadingResults = true;
        this.appService.IndexApply(this.url)
            .subscribe(data=>{
				console.log(data);
                this.indexResult = data;
                this.isLoadingResults = false;
            }, (err) => {
            	this.isLoadingResults = false;
				console.log(err);
            });
    }

    clearUrl(){
		this.appService.clearIndex()
			.subscribe(data=>{
				alert(data);
				this.url = '';
				this.indexResult.PagesIndexed = 0;
				this.indexResult.KeywordsIndexed = 0;
			});


    }
}
