import { useEffect } from "react";
import { Unity } from "react-unity-webgl";
import { LoadingOverlay } from "../components/LoadingOverlay";
import { useUnity } from "../hooks/useUnity";

export const UnityPage = () => {
  const { unityProvider, sendMessage, isLoaded, loadingProgression } = useUnity();

  useEffect(() => {
    if (isLoaded) {
      sendMessage("InitializeSceneChanger", "OnChangeCameraBySceneName", "MainGame");
    }
  }, [isLoaded, sendMessage]);

  return (
    <div
      style={{
        width: "100vw",
        height: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        backgroundColor: "#1a1a2e",
        position: "relative",
      }}
    >
      <LoadingOverlay loadingProgression={loadingProgression} isVisible={!isLoaded} />
      <Unity
        unityProvider={unityProvider}
        style={{
          width: "min(100vw, calc(100vh * 16 / 9))",
          height: "min(100vh, calc(100vw * 9 / 16))",
          aspectRatio: "16 / 9",
          visibility: isLoaded ? "visible" : "hidden",
        }}
      />
    </div>
  );
};
