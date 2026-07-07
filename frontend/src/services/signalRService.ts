import * as signalR from "@microsoft/signalr";

const baseUrl = import.meta.env.VITE_SOCKET_URL;

class SignalRService {
  private connection: signalR.HubConnection | null = null;

  public initConnection(accessToken: string): signalR.HubConnection {
    if (!this.connection) {
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(baseUrl, {
          accessTokenFactory: () => accessToken,
        })
        .withAutomaticReconnect()
        // .configureLogging(signalR.LogLevel.Warning)
        .build();
    }
    return this.connection;
  }

  public async stopConnection(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
      console.log("Đã ngắt kết nối với signalR");
    }
  }
}
export const signalRService = new SignalRService();
