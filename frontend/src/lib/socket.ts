import { io, Socket } from "socket.io-client";

// 環境変数からSocket.IOサーバーのURLを取得
const SOCKET_URL = import.meta.env.VITE_SOCKET_URL || "http://localhost:3001";

// Socket.IOインスタンスのシングルトン
let socket: Socket | null = null;

/**
 * Socket.IOインスタンスを取得します。
 * 既に接続されている場合は既存のインスタンスを返し、
 * 接続されていない場合は新しく接続します。
 *
 * @returns Socket.IOインスタンス
 */
export const getSocket = (): Socket => {
  if (!socket) {
    socket = io(SOCKET_URL, {
      transports: ["websocket", "polling"],
      autoConnect: true,
    });

    // 接続イベントのログ（開発時のみ）
    if (import.meta.env.DEV) {
      socket.on("connect", () => {
        console.log("Socket.IO connected:", socket?.id);
      });

      socket.on("disconnect", () => {
        console.log("Socket.IO disconnected");
      });

      socket.on("connect_error", (error) => {
        console.error("Socket.IO connection error:", error);
      });
    }
  }

  return socket;
};

/**
 * Socket.IO接続を切断します。
 */
export const disconnectSocket = (): void => {
  if (socket) {
    socket.disconnect();
    socket = null;
  }
};

/**
 * ルームに参加します。
 * サーバー側で`socket.on('join-room', ...)`を実装する必要があります。
 *
 * @param roomId - 参加するルームID
 */
export const joinRoom = (roomId: string): void => {
  const socketInstance = getSocket();
  socketInstance.emit("join-room", roomId);

  if (import.meta.env.DEV) {
    console.log("Joining room:", roomId);
  }
};

/**
 * ルームから退出します。
 * サーバー側で`socket.on('leave-room', ...)`を実装する必要があります。
 *
 * @param roomId - 退出するルームID
 */
export const leaveRoom = (roomId: string): void => {
  const socketInstance = getSocket();
  socketInstance.emit("leave-room", roomId);

  if (import.meta.env.DEV) {
    console.log("Leaving room:", roomId);
  }
};

// デフォルトエクスポート（直接使えるように）
export default getSocket;
