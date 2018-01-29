import { NgModule } from '@angular/core';

import {Routes, RouterModule} from '@angular/router';
import { IndexComponent } from '../components/index/index.component';
import { SearchComponent } from '../components/search/search.component';

const appRoutes: Routes = [
    {
        path: 'search',component: SearchComponent, data: {title: 'Search'}

    },{
        path: 'index',
        component: IndexComponent,
        data: {title: 'Indexation'}
    },{
        path: '',
        redirectTo: '/index',
        pathMatch: 'full'
    }
];

@NgModule({
    imports: [RouterModule.forRoot(appRoutes)],
    exports: [RouterModule]
})
export class AppRouterModule { }