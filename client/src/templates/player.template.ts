import { Component, ChangeDetectorRef, Input, OnInit, OnDestroy } from '@angular/core';
import { AppSettings } from '../core/app.settings';
import { Player } from '../core/player';
import { TimelineActivity } from '../providers/timelineactivity';
import { Subscription } from 'rxjs/Subscription';
import { ZeteticaService } from '../providers/zetetica.service';

@Component({
    templateUrl: './player.template.html',
    selector: "player-template"
})
export class PlayerTemplate implements OnInit, OnDestroy {

    @Input()
    public data: Array<any>;
    public current: any;

    public fragments: Array<TimelineActivity> = new Array();
    public playing: boolean = false;
    public endedQuote: boolean = false;

    private playFragmentSub: Subscription;
    private queueFragmentSub: Subscription;
    private playTagSub: Subscription;
    private queueTagSub: Subscription;
    private playAllSub: Subscription;
    private queueAllSub: Subscription;

    index: number = 0;
    title: string;
    author: string;

    constructor(public _settings: AppSettings,
        public _player: Player,
        private _zeteticaService: ZeteticaService,
        private _detector: ChangeDetectorRef) {
        this.fragments = new Array();
        this._player.speechStarted.subscribe(speech => {
            this.playing = true;
            this.endedQuote = false;
            console.warn("started: " + speech);
            this._detector.detectChanges();
        });
        this._player.speechEnded.subscribe(speech => {
            this.playing = false;
            this.endedQuote = true;
            console.warn("ended: " + speech);
            if (this.endedQuote) {
                this.index++;
                if (this.index >= this.fragments.length) {
                    this.index = 0;
                    this._player.stop();
                    this._detector.detectChanges();
                }
                else {
                    this.playNext();
                }
            }
        });
    }

    ngOnInit(): void {
        this.playFragmentSub = Player.playFragment.subscribe(fragmentId => {
            this._zeteticaService.fragment(fragmentId).subscribe(fragment => {
                this.fragments = new Array();
                this.fragments.push(fragment);
                this.playNext();
            });
        });
        this.queueFragmentSub = Player.queueFragment.subscribe(fragmentId => {
            this._zeteticaService.fragment(fragmentId).subscribe(fragment => {
                this.fragments.push(fragment);
                this.current = this.fragments[this.index];
                this.title = this.fragments[this.index].title;
                this.author = this.fragments[this.index].author;
                let content = this.fragments[this.index].content;
                this._detector.detectChanges();
            });
        });
        this.playTagSub = Player.playTag.subscribe(tagId => {
            this._zeteticaService.fragments(41, tagId, 0).subscribe(fragments => {
                this.fragments = new Array();
                this.fragments.push(fragments.rows);
                this.playNext();
            });
        });
        this.queueTagSub = Player.queueTag.subscribe(tagId => {
            this._zeteticaService.fragments(41, tagId, 0).subscribe(fragments => {
                this.fragments.push(fragments.rows);
                this.current = this.fragments[this.index];
                this.title = this.fragments[this.index].title;
                this.author = this.fragments[this.index].author;
                let content = this.fragments[this.index].content;
                this._detector.detectChanges();
            });
        });
        this.playAllSub = Player.playAll.subscribe(acts => {
            this.fragments = new Array();
            this.fragments = this.fragments.concat(acts.filter(f => f.author));
            this.playNext();
        });
        this.queueAllSub = Player.queueAll.subscribe(acts => {
            this.fragments = this.fragments.concat(acts.filter(f => f.author));
            this.current = this.fragments[this.index];
            this.title = this.fragments[this.index].title;
            this.author = this.fragments[this.index].author;
            let content = this.fragments[this.index].content;
            this._detector.detectChanges();
        });
    }

    ngOnDestroy(): void {
        if (this.playFragmentSub) {
            this.playFragmentSub.unsubscribe();
        }
        if (this.queueFragmentSub) {
            this.queueFragmentSub.unsubscribe();
        }
        if (this.playTagSub) {
            this.playTagSub.unsubscribe();
        }
        if (this.queueTagSub) {
            this.queueTagSub.unsubscribe();
        }
        if (this.playAllSub) {
            this.playAllSub.unsubscribe();
        }
        if (this.queueAllSub) {
            this.queueAllSub.unsubscribe();
        }
    }

    playNext(): void {
        if (this.fragments && this.fragments.length > 0) {
            if (this.index >= this.fragments.length) {
                this.index = this.fragments.length - 1;
            }
            this.current = this.fragments[this.index];
            this.title = this.fragments[this.index].title;
            this.author = this.fragments[this.index].author;
            let content = this.fragments[this.index].content;
            this._zeteticaService.listen(this.current.id).subscribe(ok => { });
            this._player.speak(this.title + '. ' + content + '. ' + this.author);
            this._detector.detectChanges();
        }
    }

    goTo(i: number): void {
        this.index = i;
        if (this.index < 0) this.index = 0;
        if (this.playing) {
            this.playNext();
        }
        else {
            if (this.fragments && this.fragments.length > 0) {
                this.current = this.fragments[this.index];
                this.title = this.fragments[this.index].title;
                this.author = this.fragments[this.index].author;
            }
            this._detector.detectChanges();
        }
    }

    callPrevious(): void {
        this.index--;
        if (this.index < 0) this.index = 0;
        if (this.playing) {
            this.playNext();
        }
        else {
            if (this.fragments && this.fragments.length > 0) {
                this.current = this.fragments[this.index];
                this.title = this.fragments[this.index].title;
                this.author = this.fragments[this.index].author;
            }
            this._detector.detectChanges();
        }
    }

    callPlay(): void {
        this.playing = true;
        if (this._player.isPaused) {
            this._player.resume();
        }
        else {
            this.playNext();
        }
    }

    callPause(): void {
        this.playing = false;
        this._player.pause();
        this._detector.detectChanges();
    }

    callStop(): void {
        this.playing = false;
        this._player.stop();
        this.index = 0;
        if (this.fragments && this.fragments.length > 0) {
            this.current = this.fragments[this.index];
            this.title = this.fragments[this.index].title;
            this.author = this.fragments[this.index].author;
        }
        this._detector.detectChanges();
    }

    callNext(): void {
        this.index++;
        if (this.index >= this.fragments.length) this.index = this.fragments.length - 1;
        if (this.playing) {
            this.playNext();
        }
        else {
            if (this.fragments && this.fragments.length > 0) {
                this.current = this.fragments[this.index];
                this.title = this.fragments[this.index].title;
                this.author = this.fragments[this.index].author;
            }
            this._detector.detectChanges();
        }
    }

    public trunc(text: string, length: number): string {
        if (text && text.length > length)
            return text.substring(0, length) + "..";
        else
            return text;
    }

}