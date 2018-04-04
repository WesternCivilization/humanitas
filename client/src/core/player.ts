import { Injectable, NgZone, EventEmitter } from '@angular/core';
import { TimelineActivity } from '../providers/timelineactivity';

interface IWindow extends Window {
    SpeechSynthesisUtterance: any;
    speechSynthesis: any;
};

@Injectable()
export class Player {

    utterence: any;

    private parts: Array<string> = new Array();
    private index: number = 0;

    public isPaused: boolean = false;

    public speechStarted: EventEmitter<string> = new EventEmitter();
    public speechEnded: EventEmitter<string> = new EventEmitter();

    static playFragment: EventEmitter<string> = new EventEmitter();
    static queueFragment: EventEmitter<string> = new EventEmitter();

    static playTag: EventEmitter<string> = new EventEmitter();
    static queueTag: EventEmitter<string> = new EventEmitter();

    static playAll: EventEmitter<Array<TimelineActivity>> = new EventEmitter();
    static queueAll: EventEmitter<Array<TimelineActivity>> = new EventEmitter();

    constructor(private zone: NgZone) { }

    playAll(acts: Array<TimelineActivity>): void {
        Player.playAll.emit(acts);
    }

    queueAll(acts: Array<TimelineActivity>): void {
        Player.queueAll.emit(acts);
    }

    play(type: string, id: string): void {
        if (type == "fragment" ||
            type == "quote" ||
            type == "note" ||
            type == "question" ||
            type == "video" ||
            type == "article" ||
            type == "audio") {
            Player.playFragment.emit(id);
        }
        else {
            Player.playTag.emit(id);
        }
    }

    queue(type: string, id: string): void {
        if (type == "fragment" ||
            type == "quote" ||
            type == "note" ||
            type == "question" ||
            type == "video" ||
            type == "article" ||
            type == "audio") {
            Player.queueFragment.emit(id);
        }
        else {
            Player.queueTag.emit(id);
        }
    }

    speak(text: string): void {

        this.isPaused = false;
        this.parts = new Array();
        let part = "";
        this.index = 0;
        for (var i = 0; i < text.length; i++) {
            part = part + text[i];
            if (text[i] == '.' ||
                text[i] == ',' ||
                text[i] == ':' ||
                text[i] == '!' ||
                text[i] == '?' ||
                text[i] == '\n') {
                if (part.length > 50) {
                    this.parts.push(part);
                    part = "";
                }
            }
            else if (text[i] == ' ' && part.length > 115) {
                this.parts.push(part);
                part = "";
            }
        }
        if (part.length > 0) {
            this.parts.push(part);
            part = "";
        }

        const { SpeechSynthesisUtterance }: IWindow = <IWindow>window;
        const { speechSynthesis }: IWindow = <IWindow>window;
        const { speechStarted }: Player = this;
        const { speechEnded }: Player = this;
        const { parts }: Player = this;
        let index = 0;

        if (this.utterence && this.utterence.onstart) this.utterence.onstart = null;
        if (this.utterence && this.utterence.onend) this.utterence.onend = null;
        if (window.speechSynthesis) window.speechSynthesis.cancel();

        this.utterence = new SpeechSynthesisUtterance();
        this.utterence.text = parts[index]; // utters text
        this.utterence.lang = 'pt-BR'; // default language
        this.utterence.volume = 1; // it can be set between 0 and 1
        this.utterence.rate = 0.8; // it can be set between 0 and 1
        this.utterence.pitch = 1; // it can be set between 0 and 1  (window as any).speechSynthesis.speak(this.utterence);

        this.utterence.onstart = function (event) {
            speechStarted.emit(parts[index]);
        };

        this.utterence.onend = function (event) {
            index++;
            if (index < parts.length) {
                this.text = parts[index]; // utters text
                window.speechSynthesis.cancel();
                window.speechSynthesis.speak(this);
            }
            else {
                speechEnded.emit(text);
            }
        };

        window.speechSynthesis.cancel();
        window.speechSynthesis.speak(this.utterence);

    }

    stop(): void {
        if (this.utterence && this.utterence.onstart) this.utterence.onstart = null;
        if (this.utterence && this.utterence.onend) this.utterence.onend = null;
        window.speechSynthesis.cancel();
    }

    pause(): void {
        this.isPaused = true;
        const { speechSynthesis }: IWindow = <IWindow>window;
        const { SpeechSynthesisUtterance }: IWindow = <IWindow>window;
        this.utterence = new SpeechSynthesisUtterance();
        (window as any).speechSynthesis.pause();
    }

    resume(): void {
        this.isPaused = false;
        (window as any).speechSynthesis.resume();
    }

}