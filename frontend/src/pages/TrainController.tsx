import { Slider } from "@mui/material";
import { useEffect, useState } from "react";
import { getSocket, joinRoom } from "../lib/socket";

const ROOM_ID = "train-control-room";

export const TrainController = () => {
  const [value, setValue] = useState(0);

  useEffect(() => {
    joinRoom(ROOM_ID);

    return () => {
      // クリーンアップ（必要に応じて）
    };
  }, []);

  const handleChange = (_event: Event, newValue: number | number[]) => {
    const speed = newValue as number;
    setValue(speed);

    // WebSocketでメッセージを送信
    const socket = getSocket();
    socket.emit("message", {
      type: "train-speed",
      roomId: ROOM_ID,
      data: {
        gameObject: "TrainController",
        method: "SetTrainSpeed",
        value: speed,
      },
    });
  };

  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        gap: "20px",
        padding: "20px",
      }}
    >
      <h1>Train Controller</h1>
      <div
        style={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          gap: "10px",
        }}
      >
        <div
          style={{
            fontSize: "24px",
            fontWeight: "bold",
            minWidth: "60px",
            textAlign: "center",
          }}
        >
          {value.toFixed(1)}
        </div>
        <div
          style={{
            height: "500px",
            display: "flex",
            alignItems: "center",
          }}
        >
          <Slider
            value={value}
            onChange={handleChange}
            orientation="vertical"
            min={0}
            max={10}
            step={0.1}
            sx={{ height: "500px" }}
          />
        </div>
      </div>
    </div>
  );
};
