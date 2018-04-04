import { Component, Input, OnInit } from '@angular/core';
import { ZeteticaService } from '../providers/zetetica.service';

@Component({
    templateUrl: './score.template.html',
    selector: "score-template"
})
export class ScoreTemplate implements OnInit {

    @Input()
    public data: Array<any>;

    @Input()
    public votes: number;

    @Input()
    public score: number;

    public myVote: number;

    isFixed: boolean;
    saving: boolean;

    constructor(private _zeteticaService: ZeteticaService) {
    }

    ngOnInit(): void {
        if (this.votes && this.score) {
            this.myVote = this.score / this.votes;
        }
    }

    @Input()
    public fragmentId: string;

    mouseover(score: number): void {
        if (!this.isFixed) {
            this.myVote = score;
        }
    }

    mouseout(): void {
        if (!this.isFixed) {
            if (this.votes && this.score) {
                this.myVote = this.score / this.votes;
            }
            else {
                this.myVote = null;
            }
        }
    }

    setScore(score: number): void {
        this.isFixed = true;
        this.saving = true;
        this.myVote = score;
        this.votes = this.votes ? this.votes + 1 : 1;
        this.score = this.score ? this.score + score : score;
        this._zeteticaService.score(this.fragmentId, score)
            .subscribe(result => this.saving = false);
    }


}