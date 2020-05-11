import { Component, OnInit } from '@angular/core';
import { Zone } from 'src/app/models/zone.model';
import { ModalController, ToastController } from '@ionic/angular';
import { NgForm, NgModel } from '@angular/forms';
import { ScheduleService } from '../services/schedule.service';
import { SensorPort } from 'src/app/models/sensorPorts.model';
import { ZoneForCreate } from 'src/app/models/zoneForCreate.model';
import { VegetablesServiceService } from 'src/app/vegetables/services/vegetables-service.service';
import { Storage } from '@ionic/storage';
import { VegetablesModel } from 'src/app/models/VegetablesModel';
@Component({
  selector: 'app-modal-zone',
  templateUrl: './modal-zone.page.html',
  styleUrls: ['./modal-zone.page.scss'],
})
export class ModalZonePage implements OnInit {

  public zoneForCreate: ZoneForCreate = new ZoneForCreate();
  public ports: SensorPort;
  public systemId = 1013;
  public vegetables: VegetablesModel[] = [];
  public formValidation = true;
  public userId: number;
  constructor(
    private modalController: ModalController,
    private scheduleService: ScheduleService,
    private vegetablesService: VegetablesServiceService,
    private storage: Storage
  ) { }

  ngOnInit() {
    this.scheduleService.GetAvailablePorts(this.systemId).subscribe(port => {
      this.ports = port;
    });
    this.storage.get('userId').then(id => {
      this.userId = id;
      this.vegetablesService.GetVegetables(id).subscribe(items => {
        this.vegetables = items;
      });
    });
  }

  submit(f: NgForm) {
    this.vegetablesService.GetVegetable(this.userId, f.value.vegetable).subscribe(item => {
      console.log(item[0].startMoisture);
      const zone: ZoneForCreate = {
        name: f.value.name,
        portId: f.value.port,
        moistureStart: item[0].startMoisture,
        moistureStop: item[0].stopMoisture,
        type: 'Moisture',
        waterSwitch: true,
        userVegetableId: f.value.vegetable
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
    });

  }

  dismissModal() {
    this.modalController.dismiss();
  }


}
