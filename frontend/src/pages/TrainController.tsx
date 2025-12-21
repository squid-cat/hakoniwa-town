/** @jsxImportSource @emotion/react */
import { css } from "@emotion/react";
import { Slider } from "@mui/material";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getSocket, joinRoom } from "../lib/socket";

type ConnectionStatus = "connecting" | "connected" | "disconnected" | "error";

export const TrainController = () => {
  const { roomId } = useParams<{ roomId: string }>();
  const navigate = useNavigate();
  const [value, setValue] = useState(0);
  const [connectionStatus, setConnectionStatus] = useState<ConnectionStatus>("connecting");
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  // TrainController専用のスタイルを適用
  useEffect(() => {
    // @keyframes pulseアニメーションを定義
    const styleId = "train-controller-pulse-animation";
    if (!document.getElementById(styleId)) {
      const style = document.createElement("style");
      style.id = styleId;
      style.textContent = `
        @keyframes pulse {
          0%, 100% {
            opacity: 1;
          }
          50% {
            opacity: 0.5;
          }
        }
      `;
      document.head.appendChild(style);
    }

    // htmlとbodyのスタイルを適用
    const originalHtmlStyle = {
      margin: document.documentElement.style.margin,
      padding: document.documentElement.style.padding,
      width: document.documentElement.style.width,
      height: document.documentElement.style.height,
      overflow: document.documentElement.style.overflow,
    };

    const originalBodyStyle = {
      margin: document.body.style.margin,
      padding: document.body.style.padding,
      width: document.body.style.width,
      height: document.body.style.height,
      overflow: document.body.style.overflow,
      position: document.body.style.position,
    };

    Object.assign(document.documentElement.style, {
      margin: "0",
      padding: "0",
      width: "100%",
      height: "100%",
      overflow: "hidden",
    });

    Object.assign(document.body.style, {
      margin: "0",
      padding: "0",
      width: "100%",
      height: "100%",
      overflow: "hidden",
      position: "fixed",
    });

    return () => {
      Object.assign(document.documentElement.style, originalHtmlStyle);
      Object.assign(document.body.style, originalBodyStyle);
    };
  }, []);

  useEffect(() => {
    // roomIdが無い場合はNotFoundページにリダイレクト
    if (!roomId) {
      navigate("/notfound", { replace: true });
      return;
    }

    // roomIdがある場合はconnection（joinRoom）を実行
    const socket = getSocket();

    // 接続状態の更新
    const handleConnect = () => {
      setConnectionStatus("connected");
      setErrorMessage(null);
      console.log("TrainController: Connected to WebSocket server");
      joinRoom(roomId);
    };

    const handleDisconnect = () => {
      setConnectionStatus("disconnected");
      console.log("TrainController: Disconnected from WebSocket server");
    };

    const handleConnectError = (error: Error) => {
      setConnectionStatus("error");
      setErrorMessage(error.message || "接続に失敗しました");
      console.error("TrainController: Failed to connect to WebSocket server:", error);
    };

    // 初期状態の設定
    if (socket.connected) {
      setConnectionStatus("connected");
      joinRoom(roomId);
    } else {
      setConnectionStatus("connecting");
    }

    socket.on("connect", handleConnect);
    socket.on("disconnect", handleDisconnect);
    socket.on("connect_error", handleConnectError);

    return () => {
      socket.off("connect", handleConnect);
      socket.off("disconnect", handleDisconnect);
      socket.off("connect_error", handleConnectError);
    };
  }, [roomId, navigate]);

  const handleChange = (_event: Event, newValue: number | number[]) => {
    if (!roomId || connectionStatus !== "connected") return;

    const speed = newValue as number;
    setValue(speed);

    // WebSocketでメッセージを送信
    const socket = getSocket();
    if (socket.connected) {
      socket.emit("message", {
        type: "train-speed",
        roomId: roomId,
        data: {
          gameObject: "TrainController",
          method: "SetTrainSpeed",
          value: speed,
        },
      });
    }
  };

  const containerStyles = css`
    width: 100vw;
    height: 100dvh; /* iOS Safari対応: 動的ビューポート高さ */
    max-height: 100vh; /* フォールバック */
    position: fixed;
    top: 0;
    left: 0;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 20px;
    padding: 20px;
    box-sizing: border-box;
    overflow: hidden;
  `;

  const titleStyles = css`
    margin: 0;
    font-size: clamp(20px, 4vw, 32px);
  `;

  const contentStyles = css`
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 10px;
    flex: 1;
    width: 100%;
    max-width: 600px;
  `;

  return (
    <div css={containerStyles}>
      <h1 css={titleStyles}>Train Controller</h1>
      <div css={contentStyles}>
        {/* 接続状態表示 */}
        <div
          css={css`
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 8px;
            margin-bottom: 10px;
          `}
        >
          <div
            css={css`
              padding: 8px 16px;
              border-radius: 4px;
              background-color: ${
                connectionStatus === "connected"
                  ? "#4caf50"
                  : connectionStatus === "connecting"
                    ? "#ff9800"
                    : "#f44336"
              };
              color: white;
              font-size: 14px;
              font-weight: bold;
              display: flex;
              align-items: center;
              gap: 8px;
            `}
          >
            <span
              css={css`
                width: 8px;
                height: 8px;
                border-radius: 50%;
                background-color: white;
                animation: ${
                  connectionStatus === "connecting" ? "pulse 1.5s ease-in-out infinite" : "none"
                };
              `}
            />
            {connectionStatus === "connected"
              ? "接続済み"
              : connectionStatus === "connecting"
                ? "接続中..."
                : connectionStatus === "error"
                  ? "接続エラー"
                  : "切断中"}
          </div>
          {errorMessage && (
            <div
              css={css`
                padding: 8px 12px;
                border-radius: 4px;
                background-color: #ffebee;
                color: #c62828;
                font-size: 12px;
                max-width: 300px;
                text-align: center;
              `}
            >
              {errorMessage}
            </div>
          )}
          {roomId && (
            <div
              css={css`
                font-size: 12px;
                color: #666;
              `}
            >
              Room ID: {roomId}
            </div>
          )}
        </div>
        <div
          css={css`
            font-size: clamp(20px, 5vw, 32px);
            font-weight: bold;
            min-width: 60px;
            text-align: center;
          `}
        >
          {value.toFixed(1)}
        </div>
        <div
          css={css`
            flex: 1;
            width: 100%;
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: 200px;
          `}
        >
          <Slider
            value={value}
            onChange={handleChange}
            orientation="vertical"
            min={0}
            max={10}
            step={0.1}
            disabled={connectionStatus !== "connected"}
            sx={{
              height: "100%",
              maxHeight: "calc(100vh - 300px)",
              minHeight: "200px",
            }}
          />
        </div>
      </div>
    </div>
  );
};
