import { Unity, useUnityContext } from "react-unity-webgl";

export const UnityPage = () => {
  const { unityProvider } = useUnityContext({
    loaderUrl: "Build/public.loader.js",
    dataUrl: "Build/public.data",
    frameworkUrl: "Build/public.framework.js",
    codeUrl: "Build/public.wasm",
  });

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
