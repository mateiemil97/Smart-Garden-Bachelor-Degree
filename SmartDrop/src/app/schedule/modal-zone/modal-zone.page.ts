import { Component, OnInit } from '@angular/core';
import { Zone } from 'src/app/models/zone.model';
import { ModalController, ToastController } from '@ionic/angular';
import { NgForm, NgModel } from '@angular/forms';
import { ScheduleService } from '../services/schedule.service';
import { SensorPort } from 'src/app/models/sensorPorts.model';
import { ZoneForCreate } from 'src/app/models/zoneForCreate.model';

@Component({
  selector: 'app-modal-zone',
  templateUrl: './modal-zone.page.html',
  styleUrls: ['./modal-zone.page.scss'],
})
export class ModalZonePage implements OnInit {

  public zoneForCreate: ZoneForCreate = new ZoneForCreate();
  public ports: SensorPort;
  public systemId = 1012;

  public formValidation = true;

  constructor(
    private modalController: ModalController,
    private scheduleService: ScheduleService,
  ) { }

  ngOnInit() {
    this.scheduleService.GetAvailablePorts(this.systemId).subscribe(port => {
      this.ports = port;
    });
  }

  submit(f: NgForm) {
    const zone = {
      name: f.value.name,
      portId: f.value.port,
      moistureStart: f.value.moistureStart,
      moistureStop: f.value.moistureStop,
      type: 'Moisture',
      waterSwitch: true
    };

    if (f.valid) {
      this.scheduleService.AddZone(this.systemId, zone).subscribe(
        err => console.log('Error on creating zone'),
        () => { console.log('Succefully created'); this.modalController.dismiss(); }
      );
    } else {
      this.formValidation = false;
    }

    console.log(f.value.port);
    console.log(f.valid);
  }

  dismissModal() {
    this.modalController.dismiss();
  }


}
