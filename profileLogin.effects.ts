import { Injectable } from '@angular/core';


import { of } from 'rxjs';
import { switchMap, tap, mergeMap, map, catchError, shareReplay } from 'rxjs/operators';

import { Actions, createEffect, ofType } from '@ngrx/effects';


import * as fromProfileLogin from './profileLogin.actions';
import { AuthService } from '../shared/services/auth.service';


@Injectable()
export class ProfileLoginEffetcs {
  constructor(private actions$: Actions, private authService: AuthService) {

  }

  checkUserName$ = createEffect(() =>
    this.actions$.pipe(
      ofType(fromProfileLogin.checkUserName),
      switchMap((action) =>
        this.authService.existUsername(action.username).pipe(
          map(data => {
            if (data.ExistProfile || data.ExistXauAccount) {
              return fromProfileLogin.usernameExist();
            } else {
              return fromProfileLogin.usernameNotExist({ errorMessage: 'Utente non trovato.' });
            }
          }),
          catchError(error => of(fromProfileLogin.usernameNotExist({ errorMessage: 'Utente non trovato.' })))
        )
      )
    )
  );


  login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(fromProfileLogin.login),
      switchMap((action) =>
        this.authService.accediAccount(action.username, action.password)
      ),
      map(data => {
        if (data.access_token && data.expires_in && data.token_type) {
          return fromProfileLogin.logged(
            {
              bearerToken: data.access_token,
              expires_in: data.expires_in
            });
        }
        return fromProfileLogin.loginFailed({ errorMessage: 'Non loggato!' });
      }),
      catchError(error => of(fromProfileLogin.loginFailed({ errorMessage: 'Password errata.' })))
    )
  );


  logged$ = createEffect(() =>
    this.actions$.pipe(
      ofType(fromProfileLogin.logged),
      switchMap((action) => this.authService.retriveProfileData()),
      tap(action => console.log('retrived...', action)),
      map(profileData => fromProfileLogin.updatedProfileData({ profileData })),
      shareReplay(),
      catchError(err => {
        console.log(err);
        return of(fromProfileLogin.loginFailed({ errorMessage: ' Non siamo riusciti a connettere ll\'utente in modo corretto!' }));
      })
    ));

}
