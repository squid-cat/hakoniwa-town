import { useEffect } from "react";
import { Unity } from "react-unity-webgl";
import { useUnity } from "../hooks/useUnity";

export const UnityController = () => {
  const { unityProvider, sendMessage } = useUnity();

  useEffect(() => {
    sendMessage("InitializeSceneChanger", "OnChangeCameraBySceneName", "TrainController");
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
