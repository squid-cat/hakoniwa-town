import { useEffect, useRef } from "react";
import { Unity } from "react-unity-webgl";
import { useUnity } from "../hooks/useUnity";
import { getSocket, joinRoom } from "../lib/socket";
import { generateUUID } from "../lib/uuid";

export const UnityPage = () => {
  const { unityProvider, sendMessage } = useUnity();
  const roomIdRef = useRef<string>(generateUUID());

  // WebSocket でメッセージを受信
  useEffect(() => {
    const socket = getSocket();
    joinRoom(roomIdRef.current);

    // WebSocketでメッセージを受信
    const handleMessage = (data: {
      type: string;
      roomId: string;
      data: {
        gameObject: string;
        method: string;
        value: number;
      };
    }) => {
      if (data.type === "train-speed" && data.roomId === roomIdRef.current) {
        // Unityにメッセージを送信
        sendMessage(data.data.gameObject, data.data.method, data.data.value);
      }
    };

    socket.on("message", handleMessage);

    return () => {
      socket.off("message", handleMessage);
    };
  }, [sendMessage]);

  // QRコードを生成
  useEffect(() => {
    const baseUrl = window.location.origin;
    const controllerUrl = `${baseUrl}/controller/${roomIdRef.current}`;
    sendMessage("TrainQRcodeController", "GenerateQRCode", controllerUrl);
  }, [sendMessage]);

  return (
    <div
      style={{
        width: "100vw",
        height: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
      }}
    >
      <Unity
        unityProvider={unityProvider}
        style={{
          width: "100%",
          height: "100%",
          maxWidth: "100%",
          maxHeight: "100%",
        }}
      />
    </div>
  );
};
