import { DatetimeChangeEventDetail } from '@ionic/core';

export class Schedule {
    id: number;
    systemId: number;
    temperatureMin: number;
    temperatureMax: number;
    start?: Date;
    stop?: Date;
    manual: boolean;
}
