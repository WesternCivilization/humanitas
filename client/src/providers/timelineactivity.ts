import { Option } from "ng-select/option";

export interface TimelineActivity
{

    title:string;

    content:string;

    author:string;

    createdTimeElapsed:string;

    listenTimeElapsed:string;

    totalListen:number;

    myScore:number;

    totalScore:number;

    tags:Array<Option>;
    
}