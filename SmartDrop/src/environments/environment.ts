// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  // url: 'https://smart-garden.conveyor.cloud/api',
  //url: 'http://localhost:58108/api',

  url: 'http://192.168.1.7:80/api',

  systemId: 1012,
  userId: 1002,
  firebase: {
    apiKey: "AIzaSyDs2UMaKgjhuu40uXpI8PwXFPhAgWza3yg",
    authDomain: "smart-drop-2eda9.firebaseapp.com",
    databaseURL: "https://smart-drop-2eda9.firebaseio.com",
    projectId: "smart-drop-2eda9",
    storageBucket: "smart-drop-2eda9.appspot.com",
    messagingSenderId: "846949956862",
    appId: "1:846949956862:web:2d7407f0206c4f824af9af",
    measurementId: "G-MNER964E0J"
  }
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
