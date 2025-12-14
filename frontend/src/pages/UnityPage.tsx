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
        maxWidth: "100%",
        maxHeight: "100%",
      }}
    >
      <Unity
        unityProvider={unityProvider}
        style={{
          width: "100%",
          height: "100%",
        }}
      />
    </div>
  );
};
