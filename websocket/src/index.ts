import "dotenv/config";
import { createServer } from "node:http";
import express from "express";
import { Server } from "socket.io";

const app = express();
const httpServer = createServer(app);

// 環境変数から設定を取得
const PORT = Number(process.env.PORT) || 3001;
const CORS_ORIGIN = process.env.CORS_ORIGIN || "http://localhost:3000";

const io = new Server(httpServer, {
  cors: {
    origin: CORS_ORIGIN,
    methods: ["GET", "POST"],
    credentials: true,
  },
});

// ルーム管理
const rooms = new Map<string, Set<string>>();

io.on("connection", (socket) => {
  console.log(`Socket connected: ${socket.id}`);

  // ルームに参加
  socket.on("join-room", (roomId: string) => {
    socket.join(roomId);

    // ルームの参加者を管理
    if (!rooms.has(roomId)) {
      rooms.set(roomId, new Set());
    }
    rooms.get(roomId)?.add(socket.id);

    console.log(`Socket ${socket.id} joined room: ${roomId}`);

    // ルーム内の他のクライアントに通知
    socket.to(roomId).emit("user-joined", {
      socketId: socket.id,
      roomId,
    });
  });

  // ルームから退出
  socket.on("leave-room", (roomId: string) => {
    socket.leave(roomId);
    rooms.get(roomId)?.delete(socket.id);

    console.log(`Socket ${socket.id} left room: ${roomId}`);

    // ルーム内の他のクライアントに通知
    socket.to(roomId).emit("user-left", {
      socketId: socket.id,
      roomId,
    });
  });

  // カスタムイベントのブロードキャスト（ルーム内）
  socket.on("message", (data) => {
    // データからroomIdを取得（送信元が指定したルームIDを使用）
    const roomId = data.roomId as string | undefined;

    if (roomId) {
      // 指定されたルーム内の他のクライアントに送信（送信元は除外）
      socket.to(roomId).emit("message", {
        ...data,
        socketId: socket.id,
      });
    }
  });

  // 切断時の処理
  socket.on("disconnect", () => {
    console.log(`Socket disconnected: ${socket.id}`);

    // すべてのルームから削除
    rooms.forEach((members, roomId) => {
      if (members.has(socket.id)) {
        members.delete(socket.id);
        socket.to(roomId).emit("user-left", {
          socketId: socket.id,
          roomId,
        });
      }
    });
  });
});

httpServer.listen(PORT, () => {
  console.log(`🚀 WebSocket server running on http://localhost:${PORT}`);
  console.log(`📡 CORS origin: ${CORS_ORIGIN}`);
});
