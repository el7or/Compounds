// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  defaultLanguage: 'en',
  protocol: 'http',
  serviceSubDomain_login: 'localhost:4010',
  serviceSubDomain_admin: 'localhost:55606',
  serviceSubDomain_visits: 'localhost:55427',
  serviceSubDomain_owners: 'localhost:7010',
  serviceSubDomain_ads: 'localhost:55610',
  appUrl: '/api/',
  HUB_URL:'http://localhost:55606/counterhub',
  googeMapUrl: "https://maps.googleapis.com/maps/api/js?key=AIzaSyB2cuIOUgqEcgXi1KvkXVr7mNa3ym4h7dA"
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
