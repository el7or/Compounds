import { EventEmitter, Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
} from "@microsoft/signalr";
import * as signalR from "@aspnet/signalr";
import { CurrentUserService } from './current-user.serives';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  counterHubURL = environment.HUB_URL;
  private _hubConnection!: HubConnection;
  connectionIsEstablished = false;
  connectionId!: string | null;

  addedExcelOwnersCount = new EventEmitter<number>();
  forceLogoutUser = new EventEmitter<string>();
  updatePendingListCount = new EventEmitter<boolean>();

  constructor(private currentUserService: CurrentUserService) {
    this.createConnection();
    this.registerOnServerEvents();
    this.startConnection();
  }


  private createConnection() {
    this._hubConnection = new HubConnectionBuilder()
      .withUrl(this.counterHubURL, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
        accessTokenFactory: () => String(this.currentUserService.currentUser?.accessToken || '')
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();
  }

  private startConnection(): void {
    Object.defineProperty(WebSocket, "OPEN", { value: 1 });
    this._hubConnection
      .start()
      .then(() => {
        this.connectionIsEstablished = true;
        console.log("Hub connection started");
        this.getConnectionId();
      })
      .catch((err) => {
        console.log("Error while establishing connection, retrying...");
        console.error(err);
        setTimeout(() => {
          this.startConnection();
        }, 5000);
      });
  }

  private getConnectionId() {
    this._hubConnection.invoke("GetConnectionId")
      .then((connectionId) => {
        this.connectionId = connectionId;
      });
  }

  private registerOnServerEvents(): void {
    this._hubConnection.on("UpdateAddedExcelOwnersCount", (data: number) => {
      this.addedExcelOwnersCount.emit(data);
    });
    this._hubConnection.on("ForceLogoutUser", (userId: string) => {
      this.forceLogoutUser.emit(userId);
    });
    this._hubConnection.on("UpdatePendingListCount", (data: boolean) => {
      this.updatePendingListCount.emit(data);
    });

    this._hubConnection.onclose((err) => { if (err) console.error(err) });
  }

  stopConnection(): void {
    this._hubConnection.stop().then(() => {
      this.connectionIsEstablished = false;
      console.log("Hub connection stoped");
    });
  }
}
