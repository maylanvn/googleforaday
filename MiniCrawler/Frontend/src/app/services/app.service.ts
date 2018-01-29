import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Search} from '../models/search.model';
import 'rxjs/add/operator/map';
import {Index} from "../models/index.model";

const URI = 'http://localhost:50529/Home';

@Injectable()
export class AppService {

    constructor(private httpClient: HttpClient) { }

    SearchApply(world) {
        return this.httpClient.get<Search[]>(URI + '/SearchWord/?keyword=' + world)
            .map((searches) => {
            console.log(searches);
            return searches;});
    }

    IndexApply(url) {
        return this.httpClient.get<Index>(URI + '/StartIndexProcess/?pageName=' + url)
            .map(res => res);
    }

    clearIndex(){
        return this.httpClient.post(URI+'/ClearIndex','')
            .map(res => res);
    }
}

