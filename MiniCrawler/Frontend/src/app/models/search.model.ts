export class Search {
    public Keyword: string;
    public Rank: number;
    public PageName: string;
    public Title: string;
    public PageURL: string;

    constructor( Keyword: string, Rank: number, PageName: string, Title: string, PageURL: string) {
        this.Keyword = Keyword;
        this.Rank = Rank;
        this.PageName = PageName;
        this.Title = Title;
        this.PageURL = PageURL;
    }
}