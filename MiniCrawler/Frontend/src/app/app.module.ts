import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import { HttpClientModule} from '@angular/common/http';

import { AppComponent } from './components/app/app.component';
import { IndexComponent } from './components/index/index.component';
import { SearchComponent } from './components/search/search.component';
import { SearchItemComponent } from './components/search-item/search-item.component';

import {AppRouterModule} from './modules/app-router.module';
import { AppMaterialModule } from './modules/app-material.module';

@NgModule({
    declarations: [
        AppComponent,
        IndexComponent,
        SearchComponent,
        SearchItemComponent
    ],
    imports: [
        HttpClientModule,
        BrowserModule,
        FormsModule,
        AppMaterialModule,
        AppRouterModule,
        BrowserAnimationsModule
    ],
    exports: [],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
