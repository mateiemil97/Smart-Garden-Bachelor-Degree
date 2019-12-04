export class ZoneForCreate {
    moistureStart: number;
    moistureStop: number;
    name?: string;
    type: string;
    /**
     *
     */
    constructor() {
        this.moistureStart = 0;
        this.moistureStop = 0;
        this.type = 'Moisture';
    }
}
