export class Zone {
    id: number;
    sensorId?: number;
    moistureStart: number;
    moistureStop: number;
    name?: string;
    changeState: boolean;
    waterSwitch: boolean;
    userVegetableName: string;
}
