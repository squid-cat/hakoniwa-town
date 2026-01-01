import { useEffect } from "react";
import { Unity } from "react-unity-webgl";
import { useUnity } from "../hooks/useUnity";

export const UnityPage = () => {
  const { unityProvider, sendMessage } = useUnity();

  useEffect(() => {
    sendMessage("InitializeSceneChanger", "OnChangeCameraBySceneName", "MainGame");
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
          width: "min(100vw, calc(100vh * 16 / 9))",
          height: "min(100vh, calc(100vw * 9 / 16))",
          aspectRatio: "16 / 9",
        }}
      />
    </div>
  );
};
