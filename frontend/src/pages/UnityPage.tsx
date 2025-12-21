import { useEffect } from "react";
import { Unity } from "react-unity-webgl";
import { useUnity } from "../hooks/useUnity";
import { getSocket, joinRoom } from "../lib/socket";

const ROOM_ID = "train-control-room";

export const UnityPage = () => {
  const { unityProvider, sendMessage } = useUnity();

  // WebSocket でメッセージを受信
  useEffect(() => {
    const socket = getSocket();
    joinRoom(ROOM_ID);

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
      if (data.type === "train-speed" && data.roomId === ROOM_ID) {
        // Unityにメッセージを送信
        sendMessage(data.data.gameObject, data.data.method, data.data.value);
      }
    };

    socket.on("message", handleMessage);

    return () => {
      socket.off("message", handleMessage);
    };
  }, [sendMessage]);

  useEffect(() => {
    // TODO: コントローラーと接続する url を生成する
    const controllerUrl = "https://github.com/squid-cat/hakoniwa-town";
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
